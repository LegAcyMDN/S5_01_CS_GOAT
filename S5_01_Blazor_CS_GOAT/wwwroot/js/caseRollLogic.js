window.caseRollLogic = {

    rollForItem: async function () {
        let endingPixel = 6800 + Math.floor(Math.random()*100)
        let audio = new Audio('/sounds/spin.mp3');
        let maxEndingPixel = endingPixel;
        
        // document.querySelector(".skin-in-case").forEach(sk => {
        //    
        // })
        let lastEndingPixel = endingPixel;
        let lastThresholdAudio = endingPixel;
        while (endingPixel > 1){
            let marginLeft = document.querySelector(".raffle-roller-container").style
                .marginLeft
            
            // momentum logic
            let pixelStep;
            if (maxEndingPixel === endingPixel){
                pixelStep = 50;
            } else {
                pixelStep = endingPixel/Math.abs(maxEndingPixel - endingPixel);
            }
            document.querySelector(".raffle-roller-container").style
                .marginLeft = (parseInt(marginLeft) - pixelStep) + "px"
            endingPixel -= pixelStep;
            if (lastThresholdAudio - endingPixel >= 100) {
                await audio.cloneNode().play()
                lastThresholdAudio = endingPixel
                console.log("last threshold audio: " + lastThresholdAudio)
            }
            
            await new Promise(r => setTimeout(r, 4));
        }
        
        
    },

    populateRoller: function () {

    },

    showRoller: function () {
        document.querySelector('.raffle-roller').style.visibility = 'visible';
    },

    printStuff: function () {
        console.log('Stuff');
    }

}