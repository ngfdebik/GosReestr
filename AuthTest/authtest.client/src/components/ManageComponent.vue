<template>
    <div id="app">
        <div class="container mt-3">
            <div class="row">
                <button @click="goBack" class="ml-2 btn btn-link">Вернуться</button>
            </div>
            <div class="d-flex min-vh-98 justify-content-center align-items-center row">
                <!-- Форма создания нового пользователя -->
                <div class="justify-content-md-center col-12 col-md-6 col-lg-4 mb-4">
                    <form class="row border shadow p-3 border-radius-20" style="width: 100%; max-width: 400px;" @submit.prevent="createUser">
                        <div v-if="Object.keys(newUserErrors).length > 0" class="text-danger mb-2">
                            <div v-for="error in newUserErrors" :key="error">{{ error }}</div>
                        </div>
                        <p class="lead m-0 align-self-center">Создание нового пользователя</p>

                        <label class="mb-1 mt-3" for="newLogin">Логин</label>
                        <input type="text" id="newLogin" v-model="newUser.login" class="form-control" :class="{'is-invalid': newUserErrors.login}">
                        <span v-if="newUserErrors.login" class="text-danger mb-2">{{ newUserErrors.login }}</span>

                        <label class="mb-1 mt-2" for="newPassword">Пароль</label>
                        <input type="password" id="newPassword" v-model="newUser.password" class="form-control" :class="{'is-invalid': newUserErrors.password}">
                        <span v-if="newUserErrors.password" class="text-danger mb-2">{{ newUserErrors.password }}</span>

                        <label class="mb-1 mt-2" for="newConfirmPassword">Подтвердите пароль</label>
                        <input type="password" id="newConfirmPassword" v-model="newUser.confirmPassword" class="form-control" :class="{'is-invalid': newUserErrors.confirmPassword}">
                        <span v-if="newUserErrors.confirmPassword" class="text-danger mb-2">{{ newUserErrors.confirmPassword }}</span>

                        <label class="mb-1 mt-2" for="newRole">Роль</label>
                        <select id="newRole" v-model="newUser.selectedRole" class="form-select" :class="{'is-invalid': newUserErrors.selectedRole}">
                            <option value="">Выберите роль</option>
                            <option v-for="role in roles" :key="role.Value" :value="role.Value">{{ role.Text }}</option>
                        </select>
                        <span v-if="newUserErrors.selectedRole" class="text-danger mb-2">{{ newUserErrors.selectedRole }}</span>

                        <label class="mb-1 mt-2" for="newFullName">ФИО</label>
                        <input type="text" id="newFullName" v-model="newUser.fullName" class="form-control mb-3">
                        
                        <button type="submit" class="btn btn-primary" :disabled="loading">Создать</button>
                        
                        <transition name="fade">
                            <div v-if="userCreateStatusMessage" class="status-message mt-2" 
                                 :class="userCreateStatusMessage.includes('успеш') ? 'alert alert-success' : 'alert alert-danger'">
                                {{ userCreateStatusMessage }}
                            </div>
                        </transition>
                    </form>
                </div>

                <!-- Форма редактирования существующего пользователя -->
                <div class="justify-content-md-center col-12 col-md-6 col-lg-4">
                    <div class="row border shadow p-3 border-radius-20" style="width: 100%; max-width: 400px;">
                        <p class="lead m-0 align-self-center">Изменение существующих пользователей</p>
                        
                        <div class="col-12 mt-3">
                            <label class="mb-1" for="existingUserSelect">Пользователь</label>
                            <select id="existingUserSelect" v-model="selectedExistingUser" class="form-select mb-2" style="width:100%">
                                <option value="">Выберите пользователя</option>
                                <option v-for="user in existingUsers" :key="user.Value" :value="user.Value">{{ user.Text }}</option>
                            </select>
                            
                            <button type="button" @click="loadUser" class="btn btn-primary" style="width:100%" 
                                    :disabled="!selectedExistingUser || loading">Редактировать</button>
                        </div>
                        
                        <form class="row mt-3" v-if="existingUser.login" @submit.prevent="updateUser">
                            <div v-if="Object.keys(existingUserErrors).length > 0" class="text-danger mb-2">
                                <div v-for="error in existingUserErrors" :key="error">{{ error }}</div>
                            </div>

                            <label class="mb-1" for="existingLogin">Логин</label>
                            <input type="text" id="existingLogin" v-model="existingUser.login" class="form-control" :class="{'is-invalid': existingUserErrors.login}">
                            <input type="hidden" v-model="existingUser.hiddenSelectedUser">
                            <span v-if="existingUserErrors.login" class="text-danger mb-2">{{ existingUserErrors.login }}</span>

                            <label class="mb-1 mt-2" for="existingPassword">Пароль</label>
                            <input type="password" id="existingPassword" v-model="existingUser.password" class="form-control" 
                                   placeholder="Оставьте пустым, если не хотите менять" :class="{'is-invalid': existingUserErrors.password}">
                            <span v-if="existingUserErrors.password" class="text-danger mb-2">{{ existingUserErrors.password }}</span>

                            <label class="mb-1 mt-2" for="existingConfirmPassword">Подтвердите пароль</label>
                            <input type="password" id="existingConfirmPassword" v-model="existingUser.confirmPassword" class="form-control" 
                                   :class="{'is-invalid': existingUserErrors.confirmPassword}">
                            <span v-if="existingUserErrors.confirmPassword" class="text-danger mb-2">{{ existingUserErrors.confirmPassword }}</span>

                            <label class="mb-1 mt-2" for="existingRole">Роль</label>
                            <select id="existingRole" v-model="existingUser.selectedRole" class="form-select" :class="{'is-invalid': existingUserErrors.selectedRole}">
                                <option value="">Выберите роль</option>
                                <option v-for="role in roles" :key="role.Value" :value="role.Value">{{ role.Text }}</option>
                            </select>
                            <span v-if="existingUserErrors.selectedRole" class="text-danger mb-2">{{ existingUserErrors.selectedRole }}</span>

                            <label class="mb-1 mt-2" for="existingFullName">ФИО</label>
                            <input type="text" id="existingFullName" v-model="existingUser.fullName" class="form-control mb-3">
                            
                            <div class="d-flex justify-content-center align-items-center flex-wrap">
                                <button type="submit" class="btn btn-success m-1" :disabled="loading">Принять изменения</button>
                                <button type="button" @click="deleteUser" class="btn btn-danger m-1" :disabled="loading">Удалить пользователя</button>
                            </div>
                            
                            <transition name="fade">
                                <div v-if="userEditStatusMessage" class="status-message mt-2" 
                                     :class="userEditStatusMessage.includes('успеш') ? 'alert alert-success' : 'alert alert-danger'">
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
                    
                    if (!validateNewUser()) return;
                    
                    loading.value = true;
                    ManageApiService.create(newUser)
                    .then(resp => {
                        userCreateStatusMessage.value = resp.message;
                        Object.keys(newUser).forEach(key => newUser[key] = '');
                    })
                    .catch (err => {
                       console.error(err);
                    })
                    loading.value = false;
                };
                
                const loadUser = async () => {
                    if (!selectedExistingUser.value) {
                        alert('Выберите пользователя для редактирования');
                        return;
                    }
                    
                    clearMessages();
                    loading.value = true;
                    ManageApiService.load(selectedExistingUser.value)
                    .then(resp => {
                        Object.assign(existingUser, {
                            login: resp.data.ExistingLoginData.Login,
                            password: '',
                            confirmPassword: '',
                            selectedRole: resp.data.ExistingLoginData.SelectedRole,
                            fullName: resp.data.ExistingLoginData.FullName,
                            hiddenSelectedUser: resp.data.ExistingLoginData.Login
                        })
                    })
                    .catch(err => {
                        console.error(err);
                    })
                        
                    loading.value = false;
                }
                
                const updateUser = async () => {
                    if (!validateExistingUser()) return;
                    
                    loading.value = true;

                    ManageApiService.update(existingUser)
                    .then(resp => {
                        userEditStatusMessage.value = resp.message;
                        //router.push(response.redirectTo)
                    })
                    .catch(err => {
                        console.error(err);
                    })

                    loading.value = false;
                };
                
                const deleteUser = async () => {
                    if (!confirm('Вы уверены, что хотите удалить этого пользователя?')) return;
                    
                    loading.value = true;

                    ManageApiService.delete(existingUser)
                    .then(resp => {
                        userEditStatusMessage.value = resp.message
                        
                        Object.keys(existingUser).forEach(key => existingUser[key] = '');
                            selectedExistingUser.value = '';
                    })
                    .catch (err => {
                        userEditStatusMessage.value = 'Ошибка удаления пользователя';
                        console.error('Ошибка удаления пользователя:', err);
                    })

                    loading.value = false;
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