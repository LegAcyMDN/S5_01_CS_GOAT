window.caseRollLogic = {

    rollForItem: async function (componentId)
    {
        try {
            console.log(`Starting roll for component: ${componentId}`);

            const container = document.querySelector(`.raffle-container-${componentId}`);

            if (!container) {
                console.error(`Container not found: .raffle-container-${componentId}`);
                return false; // Return false if failed
            }

            console.log(`Found container:`, container);

            let randomSign = Math.random() > 0.5 ? 1 : -1;
            let randomEnding = Math.floor(Math.random() * 30 * randomSign);
            let endingPixel = 6850 + randomEnding;
            let audio = new Audio('/sounds/spin.mp3');
            let maxEndingPixel = endingPixel;
            let lastThresholdAudio = endingPixel;

            while (endingPixel > 50){
                let currentMargin = container.style.marginLeft || '0px';
                let marginLeft = parseFloat(currentMargin);

                let pixelStep;
                if (maxEndingPixel === endingPixel){
                    pixelStep = 50;
                } else {
                    pixelStep = (endingPixel*10)/Math.abs(maxEndingPixel - endingPixel);
                }

                let newMargin = marginLeft - pixelStep;
                container.style.marginLeft = newMargin + "px";

                endingPixel -= pixelStep;

                if (lastThresholdAudio - endingPixel >= 100) {
                    try {
                        await audio.cloneNode().play();
                    } catch (e) {
                        console.warn('Audio play failed:', e);
                    }
                    lastThresholdAudio = endingPixel;
                }

                await new Promise(r => setTimeout(r, 2));
            }

            console.log(`Finished roll for component: ${componentId}`);
            return true; // Return true when animation completes successfully

        } catch (error) {
            console.error(`Error in rollForItem for ${componentId}:`, error);
            return false; // Return false on error
        }
    },

    populateRoller: function () {
        // Empty function
    },

    showRoller: function () {
        const roller = document.querySelector('.raffle-roller');
        if (roller) {
            roller.style.visibility = 'visible';
        }
    },

    printStuff: function () {
        console.log('Stuff');
    }
}