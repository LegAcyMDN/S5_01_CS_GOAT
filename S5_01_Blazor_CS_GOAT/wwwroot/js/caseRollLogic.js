window.caseRollLogic = {

    rollForItem: async function ()
    {
        let randomSign = Math.random() > 0.5 ? 1 : -1;
        let randomEnding = Math.floor(Math.random() * 30 * randomSign);
        let endingPixel = 6850 + randomEnding //6850
        let audio = new Audio('/sounds/spin.mp3');
        let maxEndingPixel = endingPixel;
        
        // document.querySelector(".skin-in-case").forEach(sk => {
        //    
        // })
        let lastEndingPixel = endingPixel;
        let lastThresholdAudio = endingPixel;
        while (endingPixel > 50){
            console.log("ending pixel start : " + endingPixel);
            let marginLeft = document.querySelector(".raffle-roller-container").style
                .marginLeft
            
            // momentum logic
            let pixelStep;
            if (maxEndingPixel === endingPixel){
                pixelStep = 50;
            } else {
                pixelStep = (endingPixel*10)/Math.abs(maxEndingPixel - endingPixel);
                console.log(pixelStep);
            }
            document.querySelector(".raffle-roller-container").style
                .marginLeft = (parseFloat(marginLeft) - pixelStep) + "px"
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