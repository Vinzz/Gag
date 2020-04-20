// -----------------------------------------------------------------------------------
//
//	Lightbox v2.02
//	by Lokesh Dhakar - http://www.huddletogether.com
//	3/31/06
//
//	For more information on this script, visit:
//	http://huddletogether.com/projects/lightbox2/
//
//	Licensed under the Creative Commons Attribution 2.5 License - http://creativecommons.org/licenses/by/2.5/
//	
//	Credit also due to those who have helped, inspired, and made their code available to the public.
//	Including: Scott Upton(uptonic.com), Peter-Paul Koch(quirksmode.org), Thomas Fuchs(mir.aculo.us), and others.
//
//
// -----------------------------------------------------------------------------------
/*

History of changes by ahavriluk:
07/23/06 - fixed issue with background color being green in FireFox. Made music player appear and hide depending on 
				   slideshow play mode.
07/21/06 - added color background support for music player. To change the skin look at createMusicPlayer function.
07/19/06 - added changeImageByTimer() function which helps to load the image while the entire page is loading.
07/17/06 - fixed a bug when slideshow doesn't start if navigation buttons (next/prev) were used first and slideshow=0 at start
	 - added code to remove dotted lines around images-links
	 - added code which suppose to help starting slideshow before a page is totaly loaded
*/

/*

	Table of Contents
	-----------------
	Configuration
	Global Variables

	Extending Built-in Objects	
	- Object.extend(Element)
	- Array.prototype.removeDuplicates()
	- Array.prototype.empty()

	Lightbox Class Declaration
	- initialize()
	- start()
	- changeImage()
	- resizeImageContainer()
	- showImage()
	- updateDetails()
	- updateNav()
	- enableKeyboardNav()
	- disableKeyboardNav()
	- keyboardAction()
	- preloadNeighborImages()
	- end()
	
	Miscellaneous Functions
	- getPageScroll()
	- getPageSize()
	- getKey()
	- listenKey()
	- showSelectBoxes()
	- hideSelectBoxes()
	- pause()
	- initLightbox()
	
	Function Calls
	- addLoadEvent(initLightbox)
	
	Slideshow Functions
	- toggleSlideShow()
	- startSlideShow()
	- stopSlideShow()
	- showMusicPlayer()
	- playMusic()
	- stopMusic()
	
	
	
*/
// -----------------------------------------------------------------------------------

//
//	Configuration
//
var musicPlayer = "js/playerMini.swf"
var fileLoadingImage = "images/loading.gif";		
var fileBottomNavCloseImage = "images/close1.gif";
var resizeSpeed = 7;		// controls the speed of the image resizing (1=slowest and 10=fastest)
var borderSize = 10;		//if you adjust the padding in the CSS, you will need to update this variable

//--- Slideshow options
var slideShowWidth = 600;	// -1 size slideshow window based on each image				
var slideShowHeight = 450;	// -1 size slideshow window based on each image
var SlideShowStartImage = "images/start.gif";	// Slideshow toggle button
var SlideShowStopImage = "images/stop.gif";
var slideshow = 1;   		// Set 0 if you want to disable slideshows by default
var foreverLoop = 0;		// Set 0 if want to stop on last image or Set 1 for Infinite loop feature
var loopInterval = 3000;	// image swap interval
var resize = 1;// Set 0 to disable auto-resizing
// -----------------------------------------------------------------------------------

//
//	Global Variables
//

var imageArray = new Array;
var activeImage;

if(resizeSpeed > 10){ resizeSpeed = 10;}
if(resizeSpeed < 1){ resizeSpeed = 1;}
resizeDuration = (11 - resizeSpeed) * 0.15;

var so = null;
var objSlideShowImage;
var objLightboxImage;
var objImageDataContainer;

var keyPressed = false;
var slideshowMusic = null;
var firstTime = 1;

var saveSlideshow;
var saveForeverLoop;
var saveLoopInterval;
var saveSlideShowWidth;
var saveSlideShowHeight;
// -----------------------------------------------------------------------------------

//
//	Additional methods for Element added by SU, Couloir
//	- further additions by Lokesh Dhakar (huddletogether.com)
//
Object.extend(Element, {
	getWidth: function(element) {
	   	element = $(element);
	   	return element.offsetWidth; 
	},
	setWidth: function(element,w) {
	   	element = $(element);
    	element.style.width = w +"px";
	},
	setHeight: function(element,h) {
   		element = $(element);
    	element.style.height = h +"px";
	},
	setTop: function(element,t) {
	   	element = $(element);
    	element.style.top = t +"px";
	},
	setSrc: function(element,src) {
    	element = $(element);
    	element.src = src; 
	},
	setHref: function(element,href) {
    	element = $(element);
    	element.href = href; 
	},
	setInnerHTML: function(element,content) {
		element = $(element);
		element.innerHTML = content;
	}
});

// -----------------------------------------------------------------------------------

//
//	Extending built-in Array object
//	- array.removeDuplicates()
//	- array.empty()
//
Array.prototype.removeDuplicates = function () {
	for(i = 1; i < this.length; i++){
		if(this[i][0] == this[i-1][0]){
			this.splice(i,1);
		}
	}
}

// -----------------------------------------------------------------------------------

Array.prototype.empty = function () {
	for(i = 0; i <= this.length; i++){
		this.shift();
	}
}

// -----------------------------------------------------------------------------------

//
//	Lightbox Class Declaration
//	- initialize()
//	- start()
//	- changeImage()
//	- resizeImageContainer()
//	- showImage()
//	- updateDetails()
//	- updateNav()
//	- enableKeyboardNav()
//	- disableKeyboardNav()
//	- keyboardNavAction()
//	- preloadNeighborImages()
//	- end()
//
//	Structuring of code inspired by Scott Upton (http://www.uptonic.com/)
//
var Lightbox = Class.create();

Lightbox.prototype = {
	
	// initialize()
	// Constructor runs on completion of the DOM loading. Loops through anchor tags looking for 
	// 'lightbox' references and applies onclick events to appropriate links. The 2nd section of
	// the function inserts html at the bottom of the page which is used to display the shadow 
	// overlay and the image container.
	//
	initialize: function() {	
		if (!document.getElementsByTagName){ return; }
		var anchors = document.getElementsByTagName('a');

		// loop through all anchor tags
		for (var i=0; i<anchors.length; i++){
			var anchor = anchors[i];
			
			var relAttribute = String(anchor.getAttribute('rel'));
			
			// use the string.match() method to catch 'lightbox' references in the rel attribute
			if (anchor.getAttribute('href') && (relAttribute.toLowerCase().match('lightbox'))){
				anchor.onclick = function () {myLightbox.start(this); return false;}
			}
		}

		// The rest of this code inserts html at the bottom of the page that looks similar to this:
		//
		//	<div id="overlay"></div>
		//	<div id="lightbox">
		//		<div id="outerImageContainer">
		//			<div id="imageContainer">
		//				<img id="lightboxImage">
		//				<div style="" id="hoverNav">
		//					<a href="#" id="prevLink"></a>
		//					<a href="#" id="nextLink"></a>
		//				</div>
		//				<div id="loading">
		//					<a href="#" id="loadingLink">
		//						<img src="images/loading.gif">
		//					</a>
		//				</div>
		//			</div>
		//		</div>
		//		<div id="imageDataContainer">
		//			<div id="imageData">
		//				<div id="imageDetails">
		//					<span id="caption"></span>
		//					<span id="numberDisplay"></span>
		//				</div>
		//				<div id="bottomNav">
		//					<a href="#" id="bottomNavClose">
		//						<img src="images/close.gif">
		//					</a>
		//				</div>
		//			</div>
		//		</div>
		//	</div>


		var objBody = document.getElementsByTagName("body").item(0);
		
		var objOverlay = document.createElement("div");
		objOverlay.setAttribute('id','overlay');
		objOverlay.style.display = 'none';
		objOverlay.onclick = function() { myLightbox.end(); return false; }
		objBody.appendChild(objOverlay);
		
		var objLightbox = document.createElement("div");
		objLightbox.setAttribute('id','lightbox');
		objLightbox.style.display = 'none';
		objBody.appendChild(objLightbox);
	
		var objOuterImageContainer = document.createElement("div");
		objOuterImageContainer.setAttribute('id','outerImageContainer');
		objLightbox.appendChild(objOuterImageContainer);

		var objImageContainer = document.createElement("div");
		objImageContainer.setAttribute('id','imageContainer');
		objOuterImageContainer.appendChild(objImageContainer);
	
		objLightboxImage = document.createElement("img");
		objLightboxImage.setAttribute('id','lightboxImage');
		objLightboxImage.setAttribute('width',''); //needed for proper resizing
		objLightboxImage.setAttribute('height',''); //needed for proper resizing
		objImageContainer.appendChild(objLightboxImage);
	
		var objHoverNav = document.createElement("div");
		objHoverNav.setAttribute('id','hoverNav');
		objImageContainer.appendChild(objHoverNav);
	
		var objPrevLink = document.createElement("a");
		objPrevLink.setAttribute('id','prevLink');
		objPrevLink.setAttribute('href','#');
		objPrevLink.setAttribute('onFocus', 'if (this.blur) this.blur()');
		objHoverNav.appendChild(objPrevLink);
		
		var objNextLink = document.createElement("a");
		objNextLink.setAttribute('id','nextLink');
		objNextLink.setAttribute('href','#');
		objNextLink.setAttribute('onFocus', 'if (this.blur) this.blur()');
		objHoverNav.appendChild(objNextLink);
	
		var objLoading = document.createElement("div");
		objLoading.setAttribute('id','loading');
		objImageContainer.appendChild(objLoading);
	
		var objLoadingLink = document.createElement("a");
		objLoadingLink.setAttribute('id','loadingLink');
		objLoadingLink.setAttribute('href','#');
		objLoadingLink.setAttribute('onFocus', 'if (this.blur) this.blur()');
		objLoadingLink.onclick = function() { myLightbox.end(); return false; }
		objLoading.appendChild(objLoadingLink);
	
		var objLoadingImage = document.createElement("img");
		objLoadingImage.setAttribute('src', fileLoadingImage);
		objLoadingLink.appendChild(objLoadingImage);

		objImageDataContainer = document.createElement("div");
		objImageDataContainer.setAttribute('id','imageDataContainer');
		objImageDataContainer.className = 'clearfix';
		objLightbox.appendChild(objImageDataContainer);

		var objImageData = document.createElement("div");
		objImageData.setAttribute('id','imageData');
		objImageDataContainer.appendChild(objImageData);
	
		var objImageDetails = document.createElement("div");
		objImageDetails.setAttribute('id','imageDetails');
		objImageData.appendChild(objImageDetails);
	
		var objCaption = document.createElement("span");
		objCaption.setAttribute('id','caption');
		objImageDetails.appendChild(objCaption);
	
		var objNumberDisplay = document.createElement("span");
		objNumberDisplay.setAttribute('id','numberDisplay');
		objImageDetails.appendChild(objNumberDisplay);


		//Bottom Navigation	
		var objBottomNav = document.createElement("div");
		objBottomNav.setAttribute('id','bottomNav');
		objImageData.appendChild(objBottomNav);


		var objBottomNavCloseLink = document.createElement("a");
		objBottomNavCloseLink.setAttribute('id','bottomNavClose');
		objBottomNavCloseLink.setAttribute('href','#');
		objBottomNavCloseLink.setAttribute('onFocus', 'if (this.blur) this.blur()');
		objBottomNavCloseLink.onclick = function() { myLightbox.end(); return false; }
		objBottomNav.appendChild(objBottomNavCloseLink);
	
		var objBottomNavCloseImage = document.createElement("img");
		objBottomNavCloseImage.setAttribute('src', fileBottomNavCloseImage);
		objBottomNavCloseLink.appendChild(objBottomNavCloseImage);

  			// slide show link
	 		var objSlideShowLink = document.createElement("a");
			objSlideShowLink.setAttribute('id','slideshowLink');
			objSlideShowLink.setAttribute('href','#');
			objSlideShowLink.setAttribute('onFocus', 'if (this.blur) this.blur()');
			objSlideShowLink.onclick = function() { myLightbox.toggleSlideShow(); return false; }
			objBottomNav.appendChild(objSlideShowLink);

			objSlideShowImage = document.createElement("img");
			objSlideShowImage.setAttribute('src', SlideShowStartImage);
			objSlideShowLink.appendChild(objSlideShowImage);
		
			//music player
			var objFlashPlayer = document.createElement("div");
			objFlashPlayer.setAttribute('id','flashPlayer');
			objBottomNav.appendChild(objFlashPlayer);
	},
	
	//
	//	start()
	//	Display overlay and lightbox. If image is part of a set, add siblings to imageArray.
	//
	start: function(imageLink) {	
		firstTime = 1;
		saveSlideshow = slideshow;
		saveForeverLoop = foreverLoop;
		saveLoopInterval = loopInterval;

		saveSlideShowWidth = slideShowWidth;
		saveSlideShowHeight = slideShowHeight;

		hideSelectBoxes();

		// stretch overlay to fill page and fade in
		var arrayPageSize = getPageSize();
		Element.setHeight('overlay', arrayPageSize[1]);
		new Effect.Appear('overlay', { duration: 0.2, from: 0.0, to: 0.8 });

		imageArray = [];
		imageNum = 0;		

		if (!document.getElementsByTagName){ return; }
		var anchors = document.getElementsByTagName('a');

		// if image is NOT part of a set..
		if((imageLink.getAttribute('rel') == 'lightbox')){
			// add single image to imageArray
			imageArray.push(new Array(imageLink.getAttribute('href'), imageLink.getAttribute('title')));			
		} else {
		// if image is part of a set..

			// loop through anchors, find other images in set, and add them to imageArray
			for (var i=0; i<anchors.length; i++){
				var anchor = anchors[i];
				if (anchor.getAttribute('href') && (anchor.getAttribute('rel') == imageLink.getAttribute('rel'))){
					imageArray.push(new Array(anchor.getAttribute('href'), anchor.getAttribute('title')));
					
					if (imageArray.length == 1) {
					  slideshowMusic = anchor.getAttribute('music');					  
					  if (slideshowMusic == null) {						
						  Element.hide('flashPlayer');
					  } else 
						{ Element.show('flashPlayer');	}

					  var startSlideshow = anchor.getAttribute('startslideshow');
					  if (startSlideshow != null) {
						if (startSlideshow == "false") slideshow = 0;
					  }					

					  var forever = anchor.getAttribute('forever');
					  if (forever != null) {
						if (forever == "true") foreverLoop = 1; else foreverLoop = 0;
					  }					
					  var slideDuration = anchor.getAttribute('slideDuration');
					  if (slideDuration != null) {
						loopInterval = slideDuration * 1000;
					  }					
					  var width = anchor.getAttribute('slideshowwidth');
					  if (width != null) {
						slideShowWidth = width *1;
					  }
					  var height = anchor.getAttribute('slideshowheight');
					  if (height != null) {
						slideShowHeight = height *1;
					  }
					}
					
				}
			}

			imageArray.removeDuplicates();
			while(imageArray[imageNum][0] != imageLink.getAttribute('href')) { imageNum++;}	
		}

		this.changeImageByTimer(imageNum);			
	},
	
	showLightBox: function() {
		    // calculate top offset for the lightbox and display 
	            var arrayPageSize = getPageSize();
		    var arrayPageScroll = getPageScroll();
		    var lightboxTop = arrayPageScroll[1] + (arrayPageSize[3] / 15);

		    Element.setTop('lightbox', lightboxTop);
		    Element.show('lightbox');
	},

	// changeImageByTimer()
	// changes image using timer, which prevents the loading gif from spinning 
	// until the entire page is loaded
    	changeImageByTimer: function(imageNum) {
    			activeImage = imageNum;
    			this.imageTimer = setTimeout(function() {
    			this.showLightBox();
    			this.changeImage(activeImage);
    		}.bind(this), 10);
   	 },
    
	//
	//	changeImage()
	//	Hide most elements and preload image in preparation for resizing image container.
	//
	changeImage: function(imageNum) {	
		
		activeImage = imageNum;	// update global var

		// hide elements during transition
		Element.show('loading');
		Element.hide('lightboxImage');
		Element.hide('hoverNav');
		Element.hide('prevLink');
		Element.hide('nextLink');
		

		if (firstTime == 1) {		
	  	  Element.hide('imageDataContainer');		  
		  Element.hide('numberDisplay');
		  Element.hide('slideshowLink');		
		}
			
		imgPreloader = new Image();
		
		// once image is preloaded, resize image container
		imgPreloader.onload=function(){
			Element.setSrc('lightboxImage', imageArray[activeImage][0]);

			objLightboxImage.setAttribute('width', imgPreloader.width);
			objLightboxImage.setAttribute('height', imgPreloader.height);

			if ((imageArray.length > 1) && (slideShowWidth != -1 || slideShowHeight != -1)) {
			   if (	(slideShowWidth >= imgPreloader.width) &&				
			        (slideShowHeight >= imgPreloader.height)
			      ) {			  	
			  myLightbox.resizeImageContainer(slideShowWidth, slideShowHeight);
			} else {
				  myLightbox.resizeImageAndContainer(imgPreloader.width, imgPreloader.height);
			}
			  
			} else {
			  myLightbox.resizeImageAndContainer(imgPreloader.width, imgPreloader.height);
			}
		}
		imgPreloader.src = imageArray[activeImage][0];
	},

	resizeImageAndContainer: function(imgWidth, imgHeight) {
		if(resize == 1) {//resize mod by magarnicle
			useableWidth = 0.95; // 95% of the window
			useableHeight = 0.85; // 85% of the window
			var arrayPageSize = getPageSize();
			windowWidth = arrayPageSize[2];
			windowHeight = arrayPageSize[3];
			scaleX = 1; scaleY = 1;
			if ( imgWidth > windowWidth * useableWidth ) scaleX = (windowWidth * useableWidth) / imgWidth;
			if ( imgHeight > windowHeight * useableHeight ) scaleY = (windowHeight * useableHeight) / imgHeight;
			scale = Math.min( scaleX, scaleY );
			imgWidth *= scale;
			imgHeight *= scale;

			 objLightboxImage.setAttribute('width', imgWidth);
			 objLightboxImage.setAttribute('height', imgHeight);
		}
		this.resizeImageContainer(imgWidth, imgHeight);
	},

	//
	//	resizeImageContainer()
	//
	resizeImageContainer: function( imgWidth, imgHeight) {

		// get current height and width
		this.wCur = Element.getWidth('outerImageContainer');
		this.hCur = Element.getHeight('outerImageContainer');

		// scalars based on change from old to new
		this.xScale = ((imgWidth  + (borderSize * 2)) / this.wCur) * 100;
		this.yScale = ((imgHeight  + (borderSize * 2)) / this.hCur) * 100;

		// calculate size difference between new and old image, and resize if necessary
		wDiff = (this.wCur - borderSize * 2) - imgWidth;
		hDiff = (this.hCur - borderSize * 2) - imgHeight;

		if(!( hDiff == 0)){ new Effect.Scale('outerImageContainer', this.yScale, {scaleX: false, duration: resizeDuration, queue: 'front'}); }
		if(!( wDiff == 0)){ new Effect.Scale('outerImageContainer', this.xScale, {scaleY: false, delay: resizeDuration, duration: resizeDuration}); }

		// if new and old image are same size and no scaling transition is necessary, 
		// do a quick pause to prevent image flicker.
		if((hDiff == 0) && (wDiff == 0)){
			if (navigator.appVersion.indexOf("MSIE")!=-1){ pause(250); } else { pause(100);} 
		}

		Element.setHeight('prevLink', imgHeight);
		Element.setHeight('nextLink', imgHeight);
		Element.setWidth( 'imageDataContainer', imgWidth + (borderSize * 2));

		this.showImage();
	},
	
	//
	//	showImage()
	//	Display image and begin preloading neighbors.
	//
	showImage: function(){
		Element.hide('loading');
		new Effect.Appear('lightboxImage', { duration: 0.5, queue: 'end', afterFinish: function(){ myLightbox.updateDetails(); } });
		this.preloadNeighborImages();
	},

	//
	//	updateDetails()
	//	Display caption, image number, and bottom nav.
	//
	updateDetails: function() {
	
		Element.show('caption');
		if (imageArray[activeImage][1] != '') {
			Element.setInnerHTML( 'caption', imageArray[activeImage][1]);
		} else {
			Element.setInnerHTML( 'caption', "&nbsp;");
		}

		// if image is part of set display 'Image x of x' 
		if(imageArray.length > 1){
			Element.show('numberDisplay');
			Element.setInnerHTML( 'numberDisplay', "" + eval(activeImage + 1) + " of " + imageArray.length);
		}

		if (firstTime == 1) {
                  //firstTime = 0;
		  new Effect.Parallel(
			[ new Effect.SlideDown( 'imageDataContainer', { sync: true, duration: resizeDuration + 0.25, from: 0.0, to: 1.0 }), 
			  new Effect.Appear('imageDataContainer', { sync: true, duration: 1.0 }) ], 
		 	{ duration: 0.65, afterFinish: function() { myLightbox.updateNav();} } 
		  );                   
		} else {
		  //this code was commented out because it causes the music player to restart in Firefox
//		  new Effect.Parallel(
//			[ new Effect.Appear('imageDataContainer', { sync: true, duration: 1.0 }) ], 
//		 	{ duration: 0.65, afterFinish: function() { myLightbox.updateNav();} } 
//		  );
		  myLightbox.updateNav();
		}


			if (imageArray.length > 1) {                           
			   Element.show('flashPlayer');
			   Element.show('slideshowLink');
			}else {
			   Element.hide('flashPlayer');
			   Element.hide('slideshowLink');
			}

   		           if (slideshow == 1) {
				this.startSlideShow();
			   } 

	},

	//
	//	updateNav()
	//	Display appropriate previous and next hover navigation.
	//
	updateNav: function() {

		Element.show('hoverNav');				

		// if not first image in set, display prev image button
		if(activeImage != 0){
			Element.show('prevLink');
			document.getElementById('prevLink').onclick = function() {
				if (slideshow == 1) keyPressed = true;
				myLightbox.changeImage(activeImage - 1); return false;
			}
		}

		// if not last image in set, display next image button
		if(activeImage != (imageArray.length - 1)){
			Element.show('nextLink');
			document.getElementById('nextLink').onclick = function() {
				if (slideshow == 1) keyPressed = true;
				myLightbox.changeImage(activeImage + 1); return false;
			}
		}
		
		this.enableKeyboardNav();

		if (firstTime == 1) {
		  firstTime = 0;
		  if (imageArray.length > 1 && slideshow == 1) this.showMusicPlayer();
		  if (slideshow == 1) this.playMusic(); 
		}
	},

	//
	//	enableKeyboardNav()
	//
	enableKeyboardNav: function() {
		document.onkeydown = this.keyboardAction; 
	},

	//
	//	disableKeyboardNav()
	//
	disableKeyboardNav: function() {
		document.onkeydown = '';
	},

	//
	//	keyboardAction()
	//
	keyboardAction: function(e) {
		if (e == null) { // ie
			keycode = event.keyCode;
		} else { // mozilla
			keycode = e.which;
		}

		key = String.fromCharCode(keycode).toLowerCase();

		if((key == 'x') || (key == 'o') || (key == 'c')){	// close lightbox
			myLightbox.end();
		} else if((keycode == 37) || (keycode == 38) || (key == 'p')){	// display previous image @@@@@@@@@@@@@
			if(activeImage != 0){
				if (slideshow == 1) keyPressed = true;
				myLightbox.disableKeyboardNav();							
				myLightbox.changeImage(activeImage - 1);
			}
		} else if((keycode == 39) || (keycode == 40) || (key == 'n')){	// display next image
			if(activeImage != (imageArray.length - 1)){
				if (slideshow == 1) keyPressed = true;
				myLightbox.disableKeyboardNav();				
				myLightbox.changeImage(activeImage + 1);
			}
		}


	},

	//
	//	preloadNeighborImages()
	//	Preload previous and next images.
	//
	preloadNeighborImages: function(){

		if((imageArray.length - 1) > activeImage){
			preloadNextImage = new Image();
			preloadNextImage.src = imageArray[activeImage + 1][0];
		}
		if(activeImage > 0){
			preloadPrevImage = new Image();
			preloadPrevImage.src = imageArray[activeImage - 1][0];
		}
	
	},


	//
	//	toggleSlideShow()
	//	startSlideShow()
	//	stopSlideShow()
	//	Slideshow Functions
	//

	createMusicPlayer: function() {
	      var color = Element.getStyle('imageDataContainer', 'background-color').parseColor();	      
	      obj = new SWFObject(musicPlayer, "mymovie", "75", "30", "7", color);
	      obj.addVariable("soundPath", slideshowMusic);
	      obj.addVariable("playerSkin", "5"); //skin 1-5	
	      return obj;
	},
	
	showMusicPlayer: function() {
	   if (slideshowMusic != null) {
	      Element.show('flashPlayer');
	      so = this.createMusicPlayer();	      
	      
	      so.addVariable("autoPlay", "no");
	      so.write("flashPlayer");
	   } else {
		Element.hide('flashPlayer');
		}
	},

	playMusic: function() {
	   if (slideshowMusic != null) {
		  so = this.createMusicPlayer();	      	      
		  
	      so.addVariable("autoPlay", "yes");
	      so.write("flashPlayer");
	   }
	},

	stopMusic: function() {
	   if ((slideshowMusic != null) && (so != null)) {	
		   so = this.createMusicPlayer();	     
		 
	     so.addVariable("autoPlay", "no");
	     so.write("flashPlayer");
       }
	},

	toggleSlideShow: function() {
		if(slideshow == 1) this.stopSlideShow();
		else {
		   this.playMusic();		   
		   if(activeImage == (imageArray.length-1)) {
			slideshow = 1;
			this.changeImage(0);			
		   } else {
		   	this.startSlideShow();
		   }
		}
	},

	startSlideShow: function() {
		slideshow = 1;				
		objSlideShowImage.setAttribute('src', SlideShowStopImage);		
		this.slideShowTimer = setTimeout(function() {
			if (keyPressed) {
 				keyPressed = false;
				return;
			}
			if(activeImage < (imageArray.length-1)) this.changeImage(activeImage + 1);
			else {
				if(foreverLoop) this.changeImage(0);
				else {
					this.stopMusic();
					slideshow = 0;					
					objSlideShowImage.setAttribute('src', SlideShowStartImage);					
				}
			     }	
		}.bind(this), loopInterval);
	},

	stopSlideShow: function() {
		slideshow = 0;
		objSlideShowImage.setAttribute('src', SlideShowStartImage);
		this.stopMusic();		
		if(this.slideShowTimer) {
			clearTimeout(this.slideShowTimer);
			this.slideShowTimer = null;		
			Element.setInnerHTML('flashPlayer', '');	
		}
	},

	//
	//	end()
	//
	end: function() {
		this.stopSlideShow();
		this.disableKeyboardNav();
		Element.hide('lightbox');		
		new Effect.Fade('overlay', { duration: 0.2});
		showSelectBoxes();

		slideshow = saveSlideshow;
		foreverLoop = saveForeverLoop;
		loopInterval = saveLoopInterval;

		slideShowWidth = saveSlideShowWidth;
		slideShowHeight = saveSlideShowHeight;
	}
}

// -----------------------------------------------------------------------------------

//
// getPageScroll()
// Returns array with x,y page scroll values.
// Core code from - quirksmode.org
//
function getPageScroll(){

	var yScroll;

	if (self.pageYOffset) {
		yScroll = self.pageYOffset;
	} else if (document.documentElement && document.documentElement.scrollTop){	 // Explorer 6 Strict
		yScroll = document.documentElement.scrollTop;
	} else if (document.body) {// all other Explorers
		yScroll = document.body.scrollTop;
	}

	arrayPageScroll = new Array('',yScroll) 
	return arrayPageScroll;
}

// -----------------------------------------------------------------------------------

//
// getPageSize()
// Returns array with page width, height and window width, height
// Core code from - quirksmode.org
// Edit for Firefox by pHaez
//
function getPageSize(){
	
	var xScroll, yScroll;
	
	if (window.innerHeight && window.scrollMaxY) {	
		xScroll = document.body.scrollWidth;
		yScroll = window.innerHeight + window.scrollMaxY;
	} else if (document.body.scrollHeight > document.body.offsetHeight){ // all but Explorer Mac
		xScroll = document.body.scrollWidth;
		yScroll = document.body.scrollHeight;
	} else { // Explorer Mac...would also work in Explorer 6 Strict, Mozilla and Safari
		xScroll = document.body.offsetWidth;
		yScroll = document.body.offsetHeight;
	}
	
	var windowWidth, windowHeight;
	if (self.innerHeight) {	// all except Explorer
		windowWidth = self.innerWidth;
		windowHeight = self.innerHeight;
	} else if (document.documentElement && document.documentElement.clientHeight) { // Explorer 6 Strict Mode
		windowWidth = document.documentElement.clientWidth;
		windowHeight = document.documentElement.clientHeight;
	} else if (document.body) { // other Explorers
		windowWidth = document.body.clientWidth;
		windowHeight = document.body.clientHeight;
	}	
	
	// for small pages with total height less then height of the viewport
	if(yScroll < windowHeight){
		pageHeight = windowHeight;
	} else { 
		pageHeight = yScroll;
	}

	// for small pages with total width less then width of the viewport
	if(xScroll < windowWidth){	
		pageWidth = windowWidth;
	} else {
		pageWidth = xScroll;
	}


	arrayPageSize = new Array(pageWidth,pageHeight,windowWidth,windowHeight) 
	return arrayPageSize;
}

// -----------------------------------------------------------------------------------

//
// getKey(key)
// Gets keycode. If 'x' is pressed then it hides the lightbox.
//
function getKey(e){
	if (e == null) { // ie
		keycode = event.keyCode;
	} else { // mozilla
		keycode = e.which;
	}
	key = String.fromCharCode(keycode).toLowerCase();
	
	if(key == 'x'){
	}
}

// -----------------------------------------------------------------------------------

//
// listenKey()
//
function listenKey () {	document.onkeypress = getKey; }
	
// ---------------------------------------------------

function showSelectBoxes(){
	selects = document.getElementsByTagName("select");
	for (i = 0; i != selects.length; i++) {
		selects[i].style.visibility = "visible";
	}
}

// ---------------------------------------------------

function hideSelectBoxes(){
	selects = document.getElementsByTagName("select");
	for (i = 0; i != selects.length; i++) {
		selects[i].style.visibility = "hidden";
	}
}

// ---------------------------------------------------

//
// pause(numberMillis)
// Pauses code execution for specified time. Uses busy code, not good.
// Code from http://www.faqts.com/knowledge_base/view.phtml/aid/1602
//
function pause(numberMillis) {
	var now = new Date();
	var exitTime = now.getTime() + numberMillis;
	while (true) {
		now = new Date();
		if (now.getTime() > exitTime)
			return;
	}
}

// ---------------------------------------------------



function initLightbox() { myLightbox = new Lightbox(); }
//Event.observe(window, 'load', initLightbox, false);



//the code below suppose to help starting slideshow before a page is totaly loaded
function init() {
    // quit if this function has already been called
    if (arguments.callee.done) return;

    // flag this function so we don't do the same thing twice
    arguments.callee.done = true;

    // kill the timer
    if (_timer) {
        clearInterval(_timer);
        _timer = null;
    }

    // do onload stuff
    initLightbox();

};

 

/* for Mozilla */

if (document.addEventListener) {
    document.addEventListener("DOMContentLoaded", init, false);

}

 

/* for Internet Explorer */
/*@cc_on @*/
/*@if (@_win32)
    document.write("<script id=__ie_onload defer src=javascript:void(0)></script>");
    var script = document.getElementById("__ie_onload");
    script.onreadystatechange = function() {
        if (this.readyState == "complete") {
            init(); // call the onload handler
        }
    };
/*@end @*/

 

/* for Safari */
if (/WebKit/i.test(navigator.userAgent)) { // sniff
    var _timer = setInterval(function() {
        if (/loaded|complete/.test(document.readyState)) {
            init(); // call the onload handler
        }
    }, 10);
}

 

/* for other browsers */
window.onload = init;

