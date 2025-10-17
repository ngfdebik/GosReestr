<template>
    <div id="app">
        <div class="page-container">
            <div class="flex-row">
                <button @click="goBack" class="back-btn">Вернуться</button>
            </div>
            <div class="center-layout">
                <!-- Форма создания нового пользователя -->
                <div class="form-column">
                    <form class="form-card" @submit.prevent="createUser">
                        <div v-if="Object.keys(newUserErrors).length > 0" class="error-group">
                            <div v-for="error in newUserErrors" :key="error">{{ error }}</div>
                        </div>
                        <p class="form-title">Создание нового пользователя</p>

                        <label class="input-label" for="newLogin">Логин</label>
                        <input type="text" id="newLogin" v-model="newUser.login" class="input-field" :class="{'input-error': newUserErrors.login}">
                        <span v-if="newUserErrors.login" class="error-text">{{ newUserErrors.login }}</span>

                        <label class="input-label" for="newPassword">Пароль</label>
                        <input type="password" id="newPassword" v-model="newUser.password" class="input-field" :class="{'input-error': newUserErrors.password}">
                        <span v-if="newUserErrors.password" class="error-text">{{ newUserErrors.password }}</span>

                        <label class="input-label" for="newConfirmPassword">Подтвердите пароль</label>
                        <input type="password" id="newConfirmPassword" v-model="newUser.confirmPassword" class="input-field" :class="{'input-error': newUserErrors.confirmPassword}">
                        <span v-if="newUserErrors.confirmPassword" class="error-text">{{ newUserErrors.confirmPassword }}</span>

                        <label class="input-label" for="newRole">Роль</label>
                        <select id="newRole" v-model="newUser.selectedRole" class="select-field" :class="{'input-error': newUserErrors.selectedRole}">
                            <option value="">Выберите роль</option>
                            <option v-for="role in roles" :key="role.Value" :value="role.Value">{{ role.Text }}</option>
                        </select>
                        <span v-if="newUserErrors.selectedRole" class="error-text">{{ newUserErrors.selectedRole }}</span>

                        <label class="input-label" for="newFullName">ФИО</label>
                        <input type="text" id="newFullName" v-model="newUser.fullName" class="input-field last-input">
                        
                        <button type="submit" class="primary-btn" :disabled="loading">Создать</button>
                        
                        <transition name="fade">
                            <div v-if="userCreateStatusMessage" class="status-message" 
                                 :class="userCreateStatusMessage.includes('успеш') ? 'success-alert' : 'error-alert'">
                                {{ userCreateStatusMessage }}
                            </div>
                        </transition>
                    </form>
                </div>

                <!-- Форма редактирования существующего пользователя -->
                <div class="form-column">
                    <div class="form-card">
                        <p class="form-title">Изменение существующих пользователей</p>
                        
                        <div class="user-select-section">
                            <label class="input-label" for="existingUserSelect">Пользователь</label>
                            <select id="existingUserSelect" v-model="selectedExistingUser" class="select-field full-width">
                                <option value="">Выберите пользователя</option>
                                <option v-for="user in existingUsers" :key="user.Value" :value="user.Value">{{ user.Text }}</option>
                            </select>
                            
                            <button type="button" @click="loadUser" class="primary-btn full-width-btn" 
                                    :disabled="!selectedExistingUser || loading">Редактировать</button>
                        </div>
                        
                        <form class="manage-form" v-if="existingUser.login" @submit.prevent="updateUser">
                            <div v-if="Object.keys(existingUserErrors).length > 0" class="error-group">
                                <div v-for="error in existingUserErrors" :key="error">{{ error }}</div>
                            </div>

                            <label class="input-label" for="existingLogin">Логин</label>
                            <input type="text" id="existingLogin" v-model="existingUser.login" class="input-field" :class="{'input-error': existingUserErrors.login}">
                            <input type="hidden" v-model="existingUser.hiddenSelectedUser">
                            <span v-if="existingUserErrors.login" class="error-text">{{ existingUserErrors.login }}</span>

                            <label class="input-label" for="existingPassword">Пароль</label>
                            <input type="password" id="existingPassword" v-model="existingUser.password" class="input-field" 
                                   placeholder="Оставьте пустым, если не хотите менять" :class="{'input-error': existingUserErrors.password}">
                            <span v-if="existingUserErrors.password" class="error-text">{{ existingUserErrors.password }}</span>

                            <label class="input-label" for="existingConfirmPassword">Подтвердите пароль</label>
                            <input type="password" id="existingConfirmPassword" v-model="existingUser.confirmPassword" class="input-field" 
                                   :class="{'input-error': existingUserErrors.confirmPassword}">
                            <span v-if="existingUserErrors.confirmPassword" class="error-text">{{ existingUserErrors.confirmPassword }}</span>

                            <label class="input-label" for="existingRole">Роль</label>
                            <select id="existingRole" v-model="existingUser.selectedRole" class="select-field" :class="{'input-error': existingUserErrors.selectedRole}">
                                <option value="">Выберите роль</option>
                                <option v-for="role in roles" :key="role.Value" :value="role.Value">{{ role.Text }}</option>
                            </select>
                            <span v-if="existingUserErrors.selectedRole" class="error-text">{{ existingUserErrors.selectedRole }}</span>

                            <label class="input-label" for="existingFullName">ФИО</label>
                            <input type="text" id="existingFullName" v-model="existingUser.fullName" class="input-field last-input">
                            
                            <div class="action-buttons">
                                <button type="submit" class="success-btn" :disabled="loading">Принять изменения</button>
                                <button type="button" @click="deleteUser" class="danger-btn" :disabled="loading">Удалить пользователя</button>
                            </div>
                            
                            <transition name="fade">
                                <div v-if="userEditStatusMessage" class="status-message" 
                                     :class="userEditStatusMessage.includes('успеш') ? 'success-alert' : 'error-alert'">
                                    {{ userEditStatusMessage }}
                                </div>
                            </transition>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import { ref, reactive, onMounted } from 'vue'
    import ManageApiService from '@/services/ManageApiService';
import router from '@/router/Routers';
        
        export default {
            name:"ManageComponent",
            created(){
                this.loadUsers();
            },
            setup() {
                // Состояние загрузки
                const loading = ref(false);
                
                // Данные для нового пользователя
                const newUser = reactive({
                    login: '',
                    password: '',
                    confirmPassword: '',
                    selectedRole: '',
                    fullName: ''
                });
                
                // Данные для существующего пользователя
                const existingUser = reactive({
                    login: '',
                    password: '',
                    confirmPassword: '',
                    selectedRole: '',
                    fullName: '',
                    hiddenSelectedUser: ''
                });
                
                // Ошибки валидации
                const newUserErrors = reactive({});
                const existingUserErrors = reactive({});
                
                // Сообщения статуса
                const userCreateStatusMessage = ref('');
                const userEditStatusMessage = ref('');
                
                // Выбранный пользователь для редактирования
                const selectedExistingUser = ref('');
                
                // Списки данных
                const roles = ref([]);
                
                // Имитация данных из базы
                const existingUsers = ref([]);
                // Функции
                const goBack = () => {
                    router.go(-1)
                };
                
                const clearMessages = () => {
                    userCreateStatusMessage.value = '';
                    userEditStatusMessage.value = '';
                };

                const loadUsers = async() => {
                    try {
                        const response = await ManageApiService.manage();
                        
                        // Очищаем массивы правильно
                        existingUsers.value = []; // Используем присваивание
                        roles.value = [];         // Используем присваивание
                        
                        // Добавляем данные в массивы правильно
                        if (response.ExistingUsers && Array.isArray(response.ExistingUsers)) {
                            existingUsers.value = response.ExistingUsers;
                        }
                        
                        if (response.Roles && Array.isArray(response.Roles)) {
                            roles.value = response.Roles;
                        }
                    } catch (error) {
                        console.error('Error loading users:', error);
                        userEditStatusMessage.value = 'Ошибка загрузки данных';
                    }
                };
                
                const validateNewUser = () => {
                    // Очистка предыдущих ошибок
                    Object.keys(newUserErrors).forEach(key => delete newUserErrors[key]);
                    
                    let isValid = true;
                    
                    if (!newUser.login) {
                        newUserErrors.login = 'Логин обязателен';
                        isValid = false;
                    } else if (mockUsersDb.some(u => u.login === newUser.login)) {
                        newUserErrors.login = 'Пользователь с таким логином уже существует';
                        isValid = false;
                    }
                    
                    if (!newUser.password) {
                        newUserErrors.password = 'Пароль обязателен';
                        isValid = false;
                    } else if (newUser.password.length < 6) {
                        newUserErrors.password = 'Пароль должен содержать не менее 6 символов';
                        isValid = false;
                    }
                    
                    if (newUser.password !== newUser.confirmPassword) {
                        newUserErrors.confirmPassword = 'Пароли не совпадают';
                        isValid = false;
                    }
                    
                    if (!newUser.selectedRole) {
                        newUserErrors.selectedRole = 'Роль обязательна';
                        isValid = false;
                    }
                    
                    return isValid;
                };
                
                const validateExistingUser = () => {
                    // Очистка предыдущих ошибок
                    Object.keys(existingUserErrors).forEach(key => delete existingUserErrors[key]);
                    
                    let isValid = true;
                    
                    if (!existingUser.login) {
                        existingUserErrors.login = 'Логин обязателен';
                        isValid = false;
                    }
                    
                    if (existingUser.password && existingUser.password.length < 6) {
                        existingUserErrors.password = 'Пароль должен содержать не менее 6 символов';
                        isValid = false;
                    }
                    
                    if (existingUser.password !== existingUser.confirmPassword) {
                        existingUserErrors.confirmPassword = 'Пароли не совпадают';
                        isValid = false;
                    }
                    
                    if (!existingUser.selectedRole) {
                        existingUserErrors.selectedRole = 'Роль обязательна';
                        isValid = false;
                    }
                    
                    return isValid;
                };
                const createUser = async () => {
                    clearMessages();
                    ManageApiService.create(newUser)
                    .then(resp => {
                        userCreateStatusMessage.value = resp.message;
                        Object.keys(newUser).forEach(key => newUser[key] = '');
                    })
                    .catch (err => {
                       console.error(err);
                    }).finally(() => {
                      loadUsers();
                    })
                };
                
                const loadUser = async () => {
                    if (!selectedExistingUser.value) {
                        alert('Выберите пользователя для редактирования');
                        return;
                    }
                    
                    clearMessages();
                    ManageApiService.load(selectedExistingUser.value)
                    .then(resp => {
                        Object.assign(existingUser, {
                            login: resp.ExistingLoginData.Login,
                            password: '',
                            confirmPassword: '',
                            selectedRole: resp.ExistingLoginData.SelectedRole,
                            fullName: resp.ExistingLoginData.FullName,
                            hiddenSelectedUser: resp.ExistingLoginData.Login
                        })
                    })
                    .catch(err => {
                        console.error(err);
                    })
                      .finally(() => {
                        loadUsers();
                      })
                }
                
                const updateUser = async () => {
                    if (!validateExistingUser()) return;
                    ManageApiService.update(existingUser)
                    .then(resp => {
                        userEditStatusMessage.value = resp.message;
                        //router.push(response.redirectTo)
                    })
                    .catch(err => {
                        console.error(err);
                    })
                      .finally(() => {
                        loadUsers();
                      })
                };
                
                const deleteUser = async () => {
                    if (!confirm('Вы уверены, что хотите удалить этого пользователя?')) return;
                    ManageApiService.delete(existingUser.login)
                    .then(resp => {
                        userEditStatusMessage.value = resp.message
                        
                        Object.keys(existingUser).forEach(key => existingUser[key] = '');
                            selectedExistingUser.value = '';
                    })
                    .catch (err => {
                        userEditStatusMessage.value = 'Ошибка удаления пользователя';
                        console.error('Ошибка удаления пользователя:', err);
                    })
                      .finally(() => {
                        loadUsers();
                      })

                };
                
                return {
                    loading,
                    newUser,
                    existingUser,
                    newUserErrors,
                    existingUserErrors,
                    userCreateStatusMessage,
                    userEditStatusMessage,
                    selectedExistingUser,
                    roles,
                    existingUsers,
                    loadUsers,
                    goBack,
                    createUser,
                    loadUser,
                    updateUser,
                    deleteUser
                };
            }
        };
</script>

<style>
.page-container {
    width: 100%;
    max-width: 1140px;
    padding: 0 15px;
    margin: 0 auto;
}

.flex-row {
    display: flex;
    flex-wrap: wrap;
    margin: 0 -15px;
}

.center-layout {
    display: flex;
    flex-wrap: wrap;
    min-height: 98vh;
    justify-content: center;
    align-items: center;
    margin: 0 -15px;
}

.form-column {
    flex: 0 0 100%;
    max-width: 100%;
    padding: 0 15px;
    margin-bottom: 1.5rem;
}

@media (min-width: 768px) {
    .form-column {
        flex: 0 0 50%;
        max-width: 50%;
    }
}

@media (min-width: 992px) {
    .form-column {
        flex: 0 0 33.333333%;
        max-width: 33.333333%;
    }
}

/* Form styles */
.form-card {
    width: 100%;
    max-width: 400px;
    background: white;
    border: 1px solid #dee2e6;
    border-radius: 20px;
    padding: 1rem;
    box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15);
    margin: 0 auto;
}

.form-title {
    font-size: 1.25rem;
    font-weight: 300;
    margin: 0;
    align-self: center;
}

.input-label {
    display: block;
    margin-bottom: 0.25rem;
    margin-top: 0.75rem;
    font-weight: 400;
}

.input-field, .select-field {
    display: block;
    width: 100%;
    padding: 0.375rem 0.75rem;
    font-size: 1rem;
    font-weight: 400;
    line-height: 1.5;
    color: #212529;
    background-color: #fff;
    background-clip: padding-box;
    border: 1px solid #ced4da;
    border-radius: 0.375rem;
    transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
}

.input-field:focus, .select-field:focus {
    color: #212529;
    background-color: #fff;
    border-color: #86b7fe;
    outline: 0;
    box-shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.25);
}

.input-error {
    border-color: #dc3545;
    padding-right: calc(1.5em + 0.75rem);
    background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 12 12' width='12' height='12' fill='none' stroke='%23dc3545'%3e%3ccircle cx='6' cy='6' r='4.5'/%3e%3cpath d='m5.8 3.6.4.4.4-.4'/%3e%3c/svg%3e");
    background-repeat: no-repeat;
    background-position: right calc(0.375em + 0.1875rem) center;
    background-size: calc(0.75em + 0.375rem) calc(0.75em + 0.375rem);
}

.input-error:focus {
    border-color: #dc3545;
    box-shadow: 0 0 0 0.25rem rgba(220, 53, 69, 0.25);
}

.last-input {
    margin-bottom: 1rem;
}

/* Buttons */
.back-btn {
    font-weight: 400;
    color: #0d6efd;
    text-decoration: underline;
    background-color: transparent;
    border: none;
    padding: 0;
    margin-left: 0.5rem;
    cursor: pointer;
}

.back-btn:hover {
    color: #0a58ca;
}

.primary-btn {
    display: block;
    width: 100%;
    font-weight: 400;
    line-height: 1.5;
    color: #fff;
    text-align: center;
    text-decoration: none;
    /* display: flex;
    vertical-align: middle; */
    cursor: pointer;
    user-select: none;
    background-color: #0d6efd;
    border: 1px solid #0d6efd;
    padding: 0.375rem 0.75rem;
    font-size: 1rem;
    border-radius: 0.375rem;
    transition: color 0.15s ease-in-out, background-color 0.15s ease-in-out, 
                border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
    margin-top: 0.5rem;
}

.primary-btn:hover:not(:disabled) {
    color: #fff;
    background-color: #0b5ed7;
    border-color: #0a58ca;
}

.primary-btn:disabled {
    pointer-events: none;
    opacity: 0.65;
}

.success-btn {
    display: inline-block;
    font-weight: 400;
    line-height: 1.5;
    color: #fff;
    text-align: center;
    text-decoration: none;
    vertical-align: middle;
    cursor: pointer;
    user-select: none;
    background-color: #198754;
    border: 1px solid #198754;
    padding: 0.375rem 0.75rem;
    font-size: 1rem;
    border-radius: 0.375rem;
    transition: color 0.15s ease-in-out, background-color 0.15s ease-in-out, 
                border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
    margin: 0.25rem;
}

.success-btn:hover:not(:disabled) {
    color: #fff;
    background-color: #157347;
    border-color: #146c43;
}

.danger-btn {
    display: inline-block;
    font-weight: 400;
    line-height: 1.5;
    color: #fff;
    text-align: center;
    text-decoration: none;
    vertical-align: middle;
    cursor: pointer;
    user-select: none;
    background-color: #dc3545;
    border: 1px solid #dc3545;
    padding: 0.375rem 0.75rem;
    font-size: 1rem;
    border-radius: 0.375rem;
    transition: color 0.15s ease-in-out, background-color 0.15s ease-in-out, 
                border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
    margin: 0.25rem;
}

.danger-btn:hover:not(:disabled) {
    color: #fff;
    background-color: #bb2d3b;
    border-color: #b02a37;
}

/* Utility classes */
.full-width {
    width: 100%;
}

.full-width-btn {
    width: 100%;
}

.error-group {
    color: #dc3545;
    margin-bottom: 0.5rem;
}

.error-text {
    color: #dc3545;
    font-size: 0.875rem;
    margin-bottom: 0.5rem;
    display: block;
}

/* Alert styles */
.success-alert {
    color: #0f5132;
    background-color: #d1e7dd;
    border-color: #badbcc;
    padding: 1rem;
    margin-bottom: 1rem;
    border: 1px solid transparent;
    border-radius: 0.375rem;
    margin-top: 0.5rem;
    font-size: 0.875rem;
}

.error-alert {
    color: #842029;
    background-color: #f8d7da;
    border-color: #f5c2c7;
    padding: 1rem;
    margin-bottom: 1rem;
    border: 1px solid transparent;
    border-radius: 0.375rem;
    margin-top: 0.5rem;
    font-size: 0.875rem;
}

/* Sections */
.user-select-section {
    margin-top: 1rem;
}

.manage-form {
    margin-top: 1rem;
    /*width:auto;*/
}

.action-buttons {
    display: flex;
    justify-content: center;
    align-items: center;
    flex-wrap: wrap;
}

/* Transition effects */
.fade-enter-active, .fade-leave-active {
    transition: opacity 0.5s ease;
}

.fade-enter-from, .fade-leave-to {
    opacity: 0;
}

/* Responsive */
@media (max-width: 767px) {
    .page-container {
        padding: 0 7.5px;
    }
    
    .flex-row, .center-layout {
        margin: 0 -7.5px;
    }
    
    .form-column {
        padding: 0 7.5px;
    }
    
    .action-buttons {
        flex-direction: column;
    }
    
    .success-btn, .danger-btn {
        width: 100%;
        margin: 0.25rem 0;
    }
}
</style>
