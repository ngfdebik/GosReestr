import axios from 'axios'
import store from '@/auth/store';

const client = axios.create({
    baseURL: 'https://localhost:7229/api/manage'
})

export default {
    async execute(method, resource, data = null) {
        const accessToken = localStorage.getItem('accessToken');
        return await client({
            method,
            url: resource,
            data,
            headers: {
                Authorization: `Bearer ${accessToken}`
            }
        }).then(req => {
            return req.data
        }).catch(err => {
            return err.response
        })
    },
    manage(){
        return this.execute('get', `/manage`)
    },
    create(data) {
        return this.execute('post', `/create`, data)
    },
    load(selectedExistingUser){
        return this.execute('get', `/load/${selectedExistingUser}`)
    },
    update(data) {
        return this.execute('put', `/update`, data)
    },
    delete(selectedExistingUser)  {
        return this.execute('delete', `/delete/${selectedExistingUser}`)
    }
}