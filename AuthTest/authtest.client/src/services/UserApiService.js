import axios from 'axios'
import store from '@/auth/store';

const client = axios.create({
    baseURL: 'https://localhost:7229/api/user',
    json: true
})

export default {
    async execute(method, resource, data) {
        const accessToken = localStorage.getItem('token');
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
    getCurrent(){
        return this.execute('get', `/current/${store.getters.login}`)
    },
    edit(userData){
        return this.execute('get', `/edit`, userData)
    }
}