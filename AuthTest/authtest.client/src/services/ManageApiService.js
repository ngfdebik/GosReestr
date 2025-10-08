// src/services/ManageApiService.js
import api from './TokenApi';

export default {
  manage() {
    return api.get('/manage/manage').then(response => response.data);
  },
  create(data) {
    return api.post('/manage/create', data).then(response => response.data); // ← .data
  },
  load(selectedExistingUser) {
    return api.get(`/manage/load/${selectedExistingUser}`).then(response => response.data).then(response => response.data);
  },
  update(data) {
    return api.put('/manage/update', data).then(response => response.data); // ← .data
  },
  delete(login) {
    return api.delete(`/manage/delete/${login}`).then(response => response.data); // ← .data
  }
}
