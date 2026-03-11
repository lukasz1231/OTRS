import { defineStore } from 'pinia';

export const useUserStore = defineStore('user', {
  state: () => ({
    user: null,
    isAuthenticated: false,
    isSessionChecked: false,
  }),
  
  actions: {
    setUser(userData) {
      this.user = userData;
      this.isAuthenticated = true;
    },
    
    clearUser() {
      this.user = null;
      this.isAuthenticated = false;
    },

    async fetchCurrentUser() {
      try {
        const response = await fetch('/api/Auth/me', {
          credentials: 'include',
          headers: {
            'Content-Type': 'application/json'
          }
        });
        
        if (response.ok) {
          const data = await response.json();
          this.setUser(data.user);
        } else {
          this.clearUser();
        }
      } catch (error) {
        console.error("Błąd podczas wznawiania sesji:", error);
        this.clearUser();
      } finally {
        this.isSessionChecked = true;
      }
    }
  }
});