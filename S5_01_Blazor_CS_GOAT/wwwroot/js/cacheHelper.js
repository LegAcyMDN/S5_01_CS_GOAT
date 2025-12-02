window.cacheHelper = {
    async putInCache(cacheName, url, blob) {
        try {
            const cache = await caches.open(cacheName);
            const response = new Response(blob);
            await cache.put(url, response);
            return true;
        } catch (error) {
            console.error('Error putting in cache:', error);
            return false;
        }
    },

    async putTextInCache(cacheName, url, text, contentType) {
        try {
            const cache = await caches.open(cacheName);
            const blob = new Blob([text], { type: contentType });
            const response = new Response(blob);
            await cache.put(url, response);
            return true;
        } catch (error) {
            console.error('Error putting text in cache:', error);
            return false;
        }
    },

    async getFromCache(cacheName, url) {
        try {
            const cache = await caches.open(cacheName);
            const response = await cache.match(url);
            if (response) {
                const blob = await response.blob();
                return blob;
            }
            return null;
        } catch (error) {
            console.error('Error getting from cache:', error);
            return null;
        }
    },

    async getCacheUrl(cacheName, url) {
        try {
            const cache = await caches.open(cacheName);
            const response = await cache.match(url);
            if (response) {
                const blob = await response.blob();
                return URL.createObjectURL(blob);
            }
            return null;
        } catch (error) {
            console.error('Error getting cache URL:', error);
            return null;
        }
    }
};