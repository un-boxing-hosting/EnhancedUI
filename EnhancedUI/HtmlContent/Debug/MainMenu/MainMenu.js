const {
    CefSharp,
} = window; // this is equivalent to const CefSharp = window.CefSharp
function switchImage(imgSrc) {
    console.log(document.getElementById(`html`).style.backgroundImage);
document.getElementById(`html`).style.backgroundImage = `url(${imgSrc})`
    
  
}

/*
 * Dependencies *
 this function requires the following snippets:
 JavaScript/images/switchImage

 BODY Example:
 <body onLoad="mySlideShow1.play(); mySlideShow2.play();">
 <img src="originalImage1.gif" name="slide1">
 <img src="originalImage2.gif" name="slide2">

 SCRIPT Example:
 var mySlideList1 = ['image1.gif', 'image2.gif', 'image3.gif'];
 var mySlideShow1 = new SlideShow(mySlideList1, 'slide1', 3000, "mySlideShow1");
 var mySlideList2 = ['image4.gif', 'image5.gif', 'image6.gif'];
 var mySlideShow2 = new SlideShow(mySlideList2, 'slide2', 1000, "mySlideShow2");
*/
 var MyBackGroundList = [ '../background1.png', '../background2.png', '../background3.png','../background4.png','../background5.png'];
 var MyBackGround = new SlideShow(MyBackGroundList, 'BackGround', 15000, "MyBackGround");
function SlideShow(slideList, image, speed, name) {
	console.log(`works`)

    this.slideList = slideList;
    this.image = image;
    this.speed = speed;
    this.name = name;
    this.current = 0;
    this.timer = 0;

}

SlideShow.prototype.play = SlideShow_play;

function SlideShow_play() {

 if (this.current++ === this.slideList.length - 1) {
  this.current = 0;
 }
 
 switchImage(this.slideList[this.current]);
 clearTimeout(this.timer);
 this.timer = setTimeout(this.name + '.play()', this.speed);
}
const startCefSharp = async () => {
    console.log("We are bound");
    await CefSharp.BindObjectAsync("MainMenuViewModel");
}

let continueButton = document.getElementById("continueButton");
let newGameButton = document.getElementById("newGameButton");
let loadGameButton = document.getElementById("loadGameButton");
let joinGameButton = document.getElementById("joinGameButton");
let optionsButton = document.getElementById("optionsButton");
let characterButton = document.getElementById("characterButton");
let exitButton = document.getElementById("exitButton");

startCefSharp()
    .then(() => { continueButton.addEventListener("click", (e) => window.MainMenuViewModel.ContinueLastGame()) })
    .then(() => { newGameButton.addEventListener("click", (e) => window.MainMenuViewModel.NewGame()) })
    .then(() => { loadGameButton.addEventListener("click", (e) => window.MainMenuViewModel.LoadGame()) })
    .then(() => { joinGameButton.addEventListener("click", (e) => window.MainMenuViewModel.JoinGame()) })
    .then(() => { optionsButton.addEventListener("click", (e) => window.MainMenuViewModel.Options()) })
    .then(() => { characterButton.addEventListener("click", (e) => window.MainMenuViewModel.Character()) })
    .then(() => { exitButton.addEventListener("click", (e) => window.MainMenuViewModel.Exit()) });


    
    