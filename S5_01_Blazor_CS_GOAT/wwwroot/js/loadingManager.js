window.loadingManager = {

    // Initialise le LoadingManager et connecte Blazor
    initDotNet: function (dotnetRef) {

        // Crée le LoadingManager
        const manager = new THREE.LoadingManager();

        // --- CALLBACKS DU MANAGER ---

        manager.onStart = function (url, itemsLoaded, itemsTotal) {
            // On démarre à 0%
            dotnetRef.invokeMethodAsync("UpdateProgress", 0);
        };

        manager.onProgress = function (url, itemsLoaded, itemsTotal) {
            // Calcul du pourcentage
            let percent = Math.round((itemsLoaded / itemsTotal) * 100);

            dotnetRef.invokeMethodAsync("UpdateProgress", percent);
        };

        manager.onLoad = function () {
            // 100% lorsque tout est chargé
            dotnetRef.invokeMethodAsync("UpdateProgress", 100);
        };

        manager.onError = function (url) {
            console.error("Erreur de chargement :", url);
        };

        // Stocke le manager globalement
        window._threeLoadingManager = manager;
    },

    // Active ce manager pour GLTFLoader / TextureLoader
    enableForGLTF: function () {

        if (!window._threeLoadingManager) {
            console.error("LoadingManager non initialisé.");
            return;
        }

        // Override du LoadingManager par défaut
        THREE.DefaultLoadingManager = window._threeLoadingManager;

        console.log("LoadingManager appliqué à THREE.DefaultLoadingManager.");
    },

    // Optionnel : récupérer le manager
    getManager: function () {
        return window._threeLoadingManager;
    }
};
