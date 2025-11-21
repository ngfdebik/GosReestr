import store from '@/auth/store';

export const guestGuard = (to, from, next) => {
  const requiresGuest = to.matched.some(record => record.meta.requiresGuest);
  
  if (requiresGuest || store.getters.isAuthenticated) {
    next();
  } else {
    next('/EGR');
  }
};