//WUMM Player beta 2

WUMMPlayer = {};

GamePad = {};
Wiimote = {};
Pro = {};

GamePad.Controller = null;
Wiimote.Controller = null;
Pro.Controller = null;

WUMMPlayer.Content = JSON.parse(ContentJSON);
WUMMPlayer.CurrentPath = null;
WUMMPlayer.CurrentFolder = null;
WUMMPlayer.FileFilter = null;
WUMMPlayer.FileList = null;
WUMMPlayer.CurrentFileIndex = null;
WUMMPlayer.CurrentItemIndex = null;
WUMMPlayer.RandomAudio = false;
WUMMPlayer.ImageMode = 'scale';
WUMMPlayer.ImageScale = 1.0;
WUMMPlayer.Context = null;
WUMMPlayer.AutoDimmingTV = null;
WUMMPlayer.AutoDimmingGamePad = null;
WUMMPlayer.AutoPowerDown = null;
var IsWiiU = window.nwf && nwf.system && nwf.system.isWiiU();


WUMMPlayer.ControllersInit = function() {
    if (IsWiiU) {
		if (!GamePad.Controller) {
			GamePad.Controller = nwf.input.WiiUGamePad.getController();
			GamePad.Controller.buttons.addEventListener(nwf.events.ButtonControlEvent.PRESS, GamePad.ButtonPress);
			GamePad.Controller.buttons.addEventListener(nwf.events.ButtonControlEvent.RELEASE, GamePad.ButtonRelease);
			GamePad.Controller.leftStick.addEventListener(nwf.events.MovementControlEvent.MOVE, GamePad.JoystickLeftMove);
			GamePad.Controller.rightStick.addEventListener(nwf.events.MovementControlEvent.MOVE, GamePad.JoystickRightMove);
			GamePad.Controller.touchPanel.addEventListener(nwf.events.TouchControlEvent.TOUCH_START, GamePad.TouchStart);
			GamePad.Controller.touchPanel.addEventListener(nwf.events.TouchControlEvent.TOUCH_END, GamePad.TouchEnd);
			GamePad.Controller.touchPanel.addEventListener(nwf.events.TouchControlEvent.UPDATE, GamePad.TouchUpdate);
		}
		if (!Wiimote.Controller) {
			Wiimote.Controller = nwf.input.WiiRemote.getController();
			Wiimote.Controller.buttons.addEventListener(nwf.events.ButtonControlEvent.PRESS, Wiimote.ButtonPress);
			Wiimote.Controller.buttons.addEventListener(nwf.events.ButtonControlEvent.RELEASE, Wiimote.ButtonRelease);
		}
		if (!Pro.Controller) {
			Pro.Controller = nwf.input.WiiUProController.getController();
			Pro.Controller.buttons.addEventListener(nwf.events.ButtonControlEvent.PRESS, Pro.ButtonPress);
			Pro.Controller.buttons.addEventListener(nwf.events.ButtonControlEvent.RELEASE, Pro.ButtonRelease);
			Pro.Controller.leftStick.addEventListener(nwf.events.MovementControlEvent.MOVE, Pro.JoystickLeftMove);
			Pro.Controller.rightStick.addEventListener(nwf.events.MovementControlEvent.MOVE, Pro.JoystickRightMove);
		}
	}
};

WUMMPlayer.IsVideoFile = function(file) {
	return file.ext == '.mp4';
}

WUMMPlayer.IsAudioFile = function(file) {
	return file.ext == '.m4a' || file.ext == '.ogg';
}

WUMMPlayer.IsImageFile = function(file) {
	return file.ext == '.jpg' || file.ext == '.png' || file.ext == '.gif' || file.ext == '.bmp';
}


WUMMPlayer.Init = function() {
	WUMMPlayer.ControllersInit();
	WUMMPlayer.SaveAutoDimming();
	WUMMPlayer.SaveAutoPowerDown();
	WUMMPlayer.FileFilter = 'all';
	
	if (WUMMPlayer.Content.folders.length == 0 &&
		WUMMPlayer.Content.files.length == 1 &&
		WUMMPlayer.IsVideoFile(WUMMPlayer.Content.files[0])) {
			WUMMPlayer.UpdateFolder(WUMMPlayer.Content.foldername);
			WUMMPlayer.FileList = WUMMPlayer.Content.files;
			WUMMPlayer.LoadVideo(0);
		}
	else
		WUMMPlayer.LoadFolder(WUMMPlayer.Content.foldername);
}

WUMMPlayer.LoadFolder = function(path) {
	WUMMPlayer.ResetAutoDimming();
	WUMMPlayer.ResetAutoPowerDown();
	WUMMPlayer.Context = 'showing folder';
	WUMMPlayer.CurrentFileIndex = -1;
	WUMMPlayer.CurrentItemIndex = -1;
	
	WUMMPlayer.UpdateFolder(path);

	document.getElementById('content').innerHTML = '<ul id="menu"></ul><div id="list"></div>';

	var menu = '<li id="menu_all"><a href="#" onclick="WUMMPlayer.FileFilter=\'all\';WUMMPlayer.LoadFolder(\'' + WUMMPlayer.CurrentPath + '\')"><img class="menu_img" src="all.png" /></a></li>';
		menu += '<li id="menu_video"><a href="#" onclick="WUMMPlayer.FileFilter=\'video\';WUMMPlayer.LoadFolder(\'' + WUMMPlayer.CurrentPath + '\')"><img class="menu_img" src="video.png" /></a></li>';
		menu += '<li id="menu_audio"><a href="#" onclick="WUMMPlayer.FileFilter=\'audio\';WUMMPlayer.LoadFolder(\'' + WUMMPlayer.CurrentPath + '\')"><img class="menu_img" src="audio.png" /></a></li>';
		menu += '<li id="menu_image"><a href="#" onclick="WUMMPlayer.FileFilter=\'image\';WUMMPlayer.LoadFolder(\'' + WUMMPlayer.CurrentPath + '\')"><img class="menu_img" src="image.png" /></a></li>';
	document.getElementById('menu').innerHTML = menu;
	
	var numFolders = WUMMPlayer.CurrentFolder.folders.length + 1;
	var list = '<h3 id="title">' + WUMMPlayer.CurrentPath + '</h3><ul id="item_list">';
	list += '<li id="i0" class="item"><a href="#" onclick="WUMMPlayer.LoadFolder(\'' + WUMMPlayer.CurrentPath + '/..\')">..</a></li>';
	for (var i = 0; i < WUMMPlayer.CurrentFolder.folders.length; i++) {
		list += '<li id="i' + (i + 1) + '" class="item"><a href="#" onclick="WUMMPlayer.LoadFolder(\'' + WUMMPlayer.CurrentPath + '/' + WUMMPlayer.CurrentFolder.folders[i].foldername + '\')">' + WUMMPlayer.CurrentFolder.folders[i].name + '</a></li>';
	}
	if (WUMMPlayer.FileFilter == 'video') {
		document.getElementById('menu_all').className = '';
		document.getElementById('menu_video').className = 'menu_active';
		document.getElementById('menu_audio').className = '';
		document.getElementById('menu_image').className = '';
		WUMMPlayer.FileList = WUMMPlayer.CurrentFolder.files.filter(WUMMPlayer.IsVideoFile);
		for (var i = 0; i < WUMMPlayer.FileList.length; i++) {
			list += '<li id="i' + (i + numFolders) + '" class="item"><a href="#" onclick="WUMMPlayer.LoadVideo(' + i + ')">' + WUMMPlayer.FileList[i].name + '</a></li>';
		}
	}
	else if (WUMMPlayer.FileFilter == 'audio') {
		document.getElementById('menu_all').className = '';
		document.getElementById('menu_video').className = '';
		document.getElementById('menu_audio').className = 'menu_active';
		document.getElementById('menu_image').className = '';
		WUMMPlayer.FileList = WUMMPlayer.CurrentFolder.files.filter(WUMMPlayer.IsAudioFile);
		for (var i = 0; i < WUMMPlayer.FileList.length; i++) {
			list += '<li id="i' + (i + numFolders) + '" class="item"><a href="#" onclick="WUMMPlayer.LoadAudio(' + i + ')">' + WUMMPlayer.FileList[i].name + '</a></li>';
		}
	}
	else if (WUMMPlayer.FileFilter == 'image') {
		document.getElementById('menu_all').className = '';
		document.getElementById('menu_video').className = '';
		document.getElementById('menu_audio').className = '';
		document.getElementById('menu_image').className = 'menu_active';
		WUMMPlayer.FileList = WUMMPlayer.CurrentFolder.files.filter(WUMMPlayer.IsImageFile);
		for (var i = 0; i < WUMMPlayer.FileList.length; i++) {
			list += '<li id="i' + (i + numFolders) + '" class="item"><a href="#" onclick="WUMMPlayer.LoadImage(' + i + ')">' + WUMMPlayer.FileList[i].name + '</a></li>';
		}
	}
	else {
		document.getElementById('menu_all').className = 'menu_active';
		document.getElementById('menu_video').className = '';
		document.getElementById('menu_audio').className = '';
		document.getElementById('menu_image').className = '';
		WUMMPlayer.FileList = WUMMPlayer.CurrentFolder.files;
		for (var i = 0; i < WUMMPlayer.FileList.length; i++) {
			if (WUMMPlayer.IsVideoFile(WUMMPlayer.FileList[i])) {
				list += '<li id="i' + (i + numFolders) + '" class="item"><a href="#" onclick="WUMMPlayer.LoadVideo(' + i + ')">' + WUMMPlayer.FileList[i].name + '</a></li>';
			}
			else if (WUMMPlayer.IsAudioFile(WUMMPlayer.FileList[i])) {
				list += '<li id="i' + (i + numFolders) + '" class="item"><a href="#" onclick="WUMMPlayer.LoadAudio(' + i + ')">' + WUMMPlayer.FileList[i].name + '</a></li>';
			}
			else if (WUMMPlayer.IsImageFile(WUMMPlayer.FileList[i])) {
				list += '<li id="i' + (i + numFolders) + '" class="item"><a href="#" onclick="WUMMPlayer.LoadImage(' + i + ')">' + WUMMPlayer.FileList[i].name + '</a></li>';
			}
		}
	}

	list += '</ul>';
	document.getElementById('list').innerHTML = list;
	WUMMPlayer.FitElements();
}

WUMMPlayer.UpdateFolder = function(path) {
	var folder = WUMMPlayer.Content;
	var folders = path.split("/");
	
	var level = folders.length;
	if (folders[level - 1] == '..')
		level = folders.length - 2;
		
	if (level > 1) {
		var i, j;
		for (i = 1; i < level; i++) {
			for (j = 0; j < folder.folders.length; j++) {
				if (folder.folders[j].foldername == folders[i]) {
					folder = folder.folders[j];
					break;
				}
			}
		}
	}
	
	if (folders.length > 1 && folders[folders.length - 1] == '..') {
		if (folders.length > 2)
			folders.pop();
		folders.pop();
		path = folders.join('/');
	}
	
	WUMMPlayer.CurrentPath = path;
	WUMMPlayer.CurrentFolder = folder;
}


WUMMPlayer.FitElements = function() {
	var windowWidth = window.innerWidth
	|| document.documentElement.clientWidth
	|| document.body.clientWidth;
	var windowHeight = window.innerHeight
	|| document.documentElement.clientHeight
	|| document.body.clientHeight;
	
	var list = document.getElementById("list");
	var menu = document.getElementById("menu");
	var menuImg = document.getElementsByClassName("menu_img");
	var name = document.getElementById("name");
	var backVideo = document.getElementById("backVideo");
	var backImage = document.getElementById("backImage");
	
	if (windowHeight < 720) {
		if (list) {
			list.style.marginLeft = '85px';
			list.style.fontSize = '1.8em';
		}
		if (menu) menu.style.width = '85px';
		if (menuImg) {
			for (var i = 0; i < menuImg.length; i++) {
				menuImg[i].style.width = '85px';
				menuImg[i].style.height = '85px';
			}
		}
		if (name) name.style.fontSize = '1.8em';
		if (backVideo) backVideo.style.fontSize = '1.8em';
		if (backImage) backImage.style.fontSize = '1.8em';
	}
	else if (windowHeight >= 720 && windowHeight < 1000) {
		if (list) {
			list.style.marginLeft = '128px';
			list.style.fontSize = '2.7em';
		}
		if (menu) menu.style.width = '128px';
		if (menuImg) {
			for (var i = 0; i < menuImg.length; i++) {
				menuImg[i].style.width = '128px';
				menuImg[i].style.height = '128px';
			}
		}
		if (name) name.style.fontSize = '2.7em';
		if (backVideo) backVideo.style.fontSize = '2.7em';
		if (backImage) backImage.style.fontSize = '2.7em';
	}
	else {
		if (list) {
			list.style.marginLeft = '192px';
			list.style.fontSize = '4em';
		}
		if (menu) menu.style.width = '192px';
		if (menuImg) {
			for (var i = 0; i < menuImg.length; i++) {
				menuImg[i].style.width = '192px';
				menuImg[i].style.height = '192px';
			}
		}
		if (name) name.style.fontSize = '4em';
		if (backVideo) backVideo.style.fontSize = '4em';
		if (backImage) backImage.style.fontSize = '4em';
	}
}


WUMMPlayer.LoadVideo = function(index) {
	WUMMPlayer.Context = 'playing video';
	WUMMPlayer.CurrentFileIndex = index;
	
	WUMMPlayer.DisableAutoDimming();
	WUMMPlayer.DisableAutoPowerDown();
	
	var content = '<video id="video" onmousemove="WUMMPlayer.ShowUIVideo()" onended="WUMMPlayer.NextVideo()" controls autoplay><source src="' + WUMMPlayer.CurrentPath + '/' + WUMMPlayer.FileList[index].filename + WUMMPlayer.FileList[index].ext + '" type="video/mp4">Your browser does not support the video tag.</video><h1 id="name">' + WUMMPlayer.FileList[index].name + '</h1><a id="backVideo" href="#" onclick="WUMMPlayer.LoadFolder(\'' + WUMMPlayer.CurrentPath + '\')">Back</a>';
	document.getElementById('content').innerHTML = content;
	WUMMPlayer.FitElements();
}

WUMMPlayer.NextVideo = function() {
	var i = WUMMPlayer.CurrentFileIndex + 1;
	if (i == WUMMPlayer.FileList.length)
		i = 0;
	while (!WUMMPlayer.IsVideoFile(WUMMPlayer.FileList[i])) {
		i++;
		if (i == WUMMPlayer.FileList.length)
			i = 0;
	}
	WUMMPlayer.LoadVideo(i);
}


WUMMPlayer.LoadAudio = function(index) {
	WUMMPlayer.LoadFolder(WUMMPlayer.CurrentPath);
	WUMMPlayer.Context = 'playing audio';
	WUMMPlayer.CurrentFileIndex = index;
	document.getElementById('title').innerHTML = WUMMPlayer.FileList[index].name + '<buttom id="rand_audio" onclick="WUMMPlayer.RandomAudioUpdate()"></div>';
	if (WUMMPlayer.RandomAudio)
		document.getElementById('rand_audio').innerHTML = '<img id="rand_audio_img" src="random_blue.png">';
	else 
		document.getElementById('rand_audio').innerHTML = '<img id="rand_audio_img" src="random.png">';
	var audio = '';
	if (WUMMPlayer.FileList[index].ext == '.m4a')
		audio = '<audio id="audio" onended="WUMMPlayer.NextAudio()" controls autoplay><source src="' + WUMMPlayer.CurrentPath + '/' + WUMMPlayer.FileList[index].filename + WUMMPlayer.FileList[index].ext + '" type="audio/aac">Your browser does not support the audio tag.</audio>';
	else if (WUMMPlayer.FileList[index].ext == '.ogg')
		audio = '<audio id="audio" onended="WUMMPlayer.NextAudio()" controls autoplay><source src="' + WUMMPlayer.CurrentPath + '/' + WUMMPlayer.FileList[index].filename + WUMMPlayer.FileList[index].ext + '" type="audio/ogg">Your browser does not support the audio tag.</audio>';
	document.getElementById('i' + (index + 1 + WUMMPlayer.CurrentFolder.folders.length)).innerHTML = audio;
	WUMMPlayer.FitElements();
}

WUMMPlayer.RandomAudioUpdate = function() {
	WUMMPlayer.RandomAudio = !WUMMPlayer.RandomAudio;
	if (WUMMPlayer.RandomAudio)
		document.getElementById('rand_audio').innerHTML = '<img id="rand_audio_img" src="random_blue.png">';
	else 
		document.getElementById('rand_audio').innerHTML = '<img id="rand_audio_img" src="random.png">';
}

WUMMPlayer.NextAudio = function() {
	var i;
	if (WUMMPlayer.RandomAudio) {
		i = Math.floor(Math.random() * WUMMPlayer.FileList.length);
	}
	else {
		i = WUMMPlayer.CurrentFileIndex + 1;
		if (i == WUMMPlayer.FileList.length)
			i = 0;
	}
	while (!WUMMPlayer.IsAudioFile(WUMMPlayer.FileList[i])) {
		i++;
		if (i == WUMMPlayer.FileList.length)
			i = 0;
	}
	WUMMPlayer.LoadAudio(i);
}


WUMMPlayer.LoadImage = function(index) {
	WUMMPlayer.Context = 'showing image';
	WUMMPlayer.CurrentFileIndex = index;

	var content = '<div id="image_bg" onmousemove="WUMMPlayer.ShowUIImage()"><img id="image" src="' + WUMMPlayer.CurrentPath + '/' + WUMMPlayer.FileList[index].filename + WUMMPlayer.FileList[index].ext + '" /></div><h1 id="name">' + WUMMPlayer.FileList[index].name + '</h1><a id="backImage" href="#" onclick="WUMMPlayer.LoadFolder(\'' + WUMMPlayer.CurrentPath + '\')">Back</a>';
	document.getElementById('content').innerHTML = content;
	
	WUMMPlayer.FitElements();
	WUMMPlayer.ResetImageScene();
}

WUMMPlayer.ResetImageScene = function() {
	window.scrollTo(0, 0);
	var windowWidth = window.innerWidth
	|| document.documentElement.clientWidth
	|| document.body.clientWidth;
	var windowHeight = window.innerHeight
	|| document.documentElement.clientHeight
	|| document.body.clientHeight;

	WUMMPlayer.SetImageScale(1);
	
	if (WUMMPlayer.ImageMode == 'scale') {
		var widthScale = windowWidth / WUMMPlayer.FileList[WUMMPlayer.CurrentFileIndex].width;
		var heightScale = windowHeight / WUMMPlayer.FileList[WUMMPlayer.CurrentFileIndex].height;
		if (widthScale < 1 || heightScale < 1) {
			if (widthScale < heightScale)
				WUMMPlayer.SetImageScale(widthScale);
			else
				WUMMPlayer.SetImageScale(heightScale);
		}
		WUMMPlayer.CenterImage();
	}
	else if (WUMMPlayer.ImageMode == 'width') {
		var scale = windowWidth / WUMMPlayer.FileList[WUMMPlayer.CurrentFileIndex].width;
		WUMMPlayer.SetImageScale(scale);
		document.getElementById("image").style.right = '0px';
		document.getElementById("image").style.top = '0px';
	}
}

WUMMPlayer.SetImageScale = function(scale) {
	if (scale >= 0.1 && scale <= 10.0) {
		WUMMPlayer.ImageScale = scale;
		var width = WUMMPlayer.FileList[WUMMPlayer.CurrentFileIndex].width * scale;
		var height = WUMMPlayer.FileList[WUMMPlayer.CurrentFileIndex].height * scale;
		document.getElementById("image").style.width = width + 'px';
		document.getElementById("image").style.height = height + 'px';
	}
}

WUMMPlayer.CenterImage = function() {
	var windowWidth = window.innerWidth
	|| document.documentElement.clientWidth
	|| document.body.clientWidth;
	var windowHeight = window.innerHeight
	|| document.documentElement.clientHeight
	|| document.body.clientHeight;
	
	var imageWidth = parseInt(document.getElementById("image").style.width);
	var imageHeight = parseInt(document.getElementById("image").style.height);
	var backgroundWidth = windowWidth > imageWidth ? windowWidth : imageWidth;
	var backgroundHeight = windowHeight > imageHeight ? windowHeight : imageHeight;
	
	var positionX = Math.floor((backgroundWidth - imageWidth) / 2);
	var positionY = Math.floor((backgroundHeight - imageHeight) / 2);
	document.getElementById("image").style.right = positionX + 'px';
	document.getElementById("image").style.top = positionY + 'px';
	
	document.getElementById("image_bg").style.width = backgroundWidth + 'px';
	document.getElementById("image_bg").style.height = backgroundHeight + 'px';
}

WUMMPlayer.PreviousImage = function() {
	var i = WUMMPlayer.CurrentFileIndex - 1;
	if (i == -1)
		i = WUMMPlayer.FileList.length - 1;
	while (!WUMMPlayer.IsImageFile(WUMMPlayer.FileList[i])) {
		i--;
		if (i == -1)
			i = WUMMPlayer.FileList.length - 1;
	}
	WUMMPlayer.LoadImage(i);
}

WUMMPlayer.NextImage = function() {
	var i = WUMMPlayer.CurrentFileIndex + 1;
	if (i == WUMMPlayer.FileList.length)
		i = 0;
	while (!WUMMPlayer.IsImageFile(WUMMPlayer.FileList[i])) {
		i++;
		if (i == WUMMPlayer.FileList.length)
			i = 0;
	}
	WUMMPlayer.LoadImage(i);
}


WUMMPlayer.SaveAutoDimming = function(){
    if (IsWiiU) { 
		WUMMPlayer.DimmingTV = nwf.display.DisplayManager.getInstance().getTVDisplay().dimmingEnabled;
		WUMMPlayer.DimmingGamePad = nwf.display.DisplayManager.getInstance().getGamePadDisplay().dimmingEnabled;
    }
};

WUMMPlayer.ResetAutoDimming = function() {
    if (IsWiiU) {      
		nwf.display.DisplayManager.getInstance().getTVDisplay().dimmingEnabled = WUMMPlayer.DimmingTV;
		nwf.display.DisplayManager.getInstance().getGamePadDisplay().dimmingEnabled = WUMMPlayer.DimmingGamePad;
    }
};
  
WUMMPlayer.DisableAutoDimming = function() {
    if (IsWiiU) {      
		nwf.display.DisplayManager.getInstance().getTVDisplay().dimmingEnabled = false;
		nwf.display.DisplayManager.getInstance().getGamePadDisplay().dimmingEnabled = false;
    }
};

WUMMPlayer.SaveAutoPowerDown = function() {
	if (IsWiiU) {
		WUMMPlayer.AutoPowerDown = nwf.system.APD.isEnabled;
	}
};

WUMMPlayer.ResetAutoPowerDown = function() {
	if (IsWiiU) {
		if (WUMMPlayer.AutoPowerDown) {
			try {
				nwf.system.APD.enable();
			} catch (ex) {
				
			}
		}
	}
};

WUMMPlayer.DisableAutoPowerDown = function() {
	if (IsWiiU && nwf.system.APD.isEnabled) {
		try {
			nwf.system.APD.disable();
		} catch(ex) {

		}
	}
};


WUMMPlayer.ShowUIVideo = function() {
	var name = document.getElementById('name');
	var backVideo = document.getElementById('backVideo');
	
	name.style.webkitAnimationName = 'none';
	setTimeout(function() {
        name.style.webkitAnimationName = '';
    }, 10);
	
	backVideo.style.webkitAnimationName = 'none';
	setTimeout(function() {
        backVideo.style.webkitAnimationName = '';
    }, 10);
}

WUMMPlayer.ShowUIImage = function() {
	var name = document.getElementById('name');
	var backImage = document.getElementById('backImage');
	
	name.style.webkitAnimationName = 'none';
	setTimeout(function() {
        name.style.webkitAnimationName = '';
    }, 10);
	
	backImage.style.webkitAnimationName = 'none';
	setTimeout(function() {
        backImage.style.webkitAnimationName = '';
    }, 10);
}


GamePad.Connected = function() {
	return IsWiiU && GamePad.Controller && GamePad.Controller.connected;
};

Wiimote.Connected = function() {
	return IsWiiU && Wiimote.Controller && Wiimote.Controller.connected;
};

Pro.Connected = function() {
	return IsWiiU && Pro.Controller && Pro.Controller.connected;
};


GamePad.ButtonPress = function(event) {
	if (WUMMPlayer.Context == 'showing image') {
		if (event.button == nwf.input.ControllerButton.GAMEPAD_UP) {
			var positionY = parseInt(document.getElementById("image").style.top) + 20;
			document.getElementById("image").style.top = positionY + 'px';
		}
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_DOWN) {
			var positionY = parseInt(document.getElementById("image").style.top) - 20;
			document.getElementById("image").style.top = positionY + 'px';
		}
		if (event.button == nwf.input.ControllerButton.GAMEPAD_LEFT) {
			var positionX = parseInt(document.getElementById("image").style.right) + 20;
			document.getElementById("image").style.right = positionX + 'px';
		}
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_RIGHT) {
			var positionX = parseInt(document.getElementById("image").style.right) - 20;
			document.getElementById("image").style.right = positionX + 'px';
		}
	}
};

GamePad.ButtonRelease = function(event) {
	if (WUMMPlayer.Context == 'showing folder' || WUMMPlayer.Context == 'playing audio') {
		if (event.button == nwf.input.ControllerButton.GAMEPAD_DOWN) {
			var numItems = 1 + WUMMPlayer.CurrentFolder.folders.length + WUMMPlayer.FileList.length;
			for (var i = 0; i < numItems; i++)
				document.getElementById('i' + i).className = 'item';
			WUMMPlayer.CurrentItemIndex++;
			if (WUMMPlayer.CurrentItemIndex >= numItems)
				WUMMPlayer.CurrentItemIndex = 0;
			document.getElementById('i' + WUMMPlayer.CurrentItemIndex).className = 'item selected';
		}
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_UP) {
			var numItems = 1 + WUMMPlayer.CurrentFolder.folders.length + WUMMPlayer.FileList.length;
			for (var i = 0; i < numItems; i++)
				document.getElementById('i' + i).className = 'item';
			WUMMPlayer.CurrentItemIndex--;
			if (WUMMPlayer.CurrentItemIndex < 0)
				WUMMPlayer.CurrentItemIndex = numItems - 1;
			document.getElementById('i' + WUMMPlayer.CurrentItemIndex).className = 'item selected';
		}
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_A && WUMMPlayer.CurrentItemIndex != -1) {
			if (WUMMPlayer.CurrentItemIndex == 0)
				WUMMPlayer.LoadFolder(WUMMPlayer.CurrentPath + '/..');
			else if (WUMMPlayer.CurrentFolder.folders.length > 0 &&
				WUMMPlayer.CurrentItemIndex - 1 < WUMMPlayer.CurrentFolder.folders.length)
				WUMMPlayer.LoadFolder(WUMMPlayer.CurrentPath + '/' + WUMMPlayer.CurrentFolder.folders[WUMMPlayer.CurrentItemIndex - 1].foldername);
			else if (WUMMPlayer.FileList.length > 0 &&
				WUMMPlayer.CurrentItemIndex - 1 - WUMMPlayer.CurrentFolder.folders.length < WUMMPlayer.FileList.length) {
				var index = WUMMPlayer.CurrentItemIndex - 1 - WUMMPlayer.CurrentFolder.folders.length;
				if (WUMMPlayer.IsVideoFile(WUMMPlayer.FileList[index]))
					WUMMPlayer.LoadVideo(index);
				else if (WUMMPlayer.IsAudioFile(WUMMPlayer.FileList[index]))
					WUMMPlayer.LoadAudio(index);
				else if (WUMMPlayer.IsImageFile(WUMMPlayer.FileList[index]))
					WUMMPlayer.LoadImage(index);
			}
		}
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_B) {
			if (WUMMPlayer.Context == 'showing folder')
				WUMMPlayer.LoadFolder(WUMMPlayer.CurrentPath + '/..');
			else if (WUMMPlayer.Context == 'playing audio')
				WUMMPlayer.LoadFolder(WUMMPlayer.CurrentPath);
		}
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_X && WUMMPlayer.Context == 'playing audio') {
			WUMMPlayer.RandomAudioUpdate();
		}
	}
	else if (WUMMPlayer.Context == 'playing video') {
		if (event.button == nwf.input.ControllerButton.GAMEPAD_B) {
			WUMMPlayer.LoadFolder(WUMMPlayer.CurrentPath);
		}
	}
	else if (WUMMPlayer.Context == 'showing image') {
		if (event.button == nwf.input.ControllerButton.GAMEPAD_L) {
			WUMMPlayer.PreviousImage();
		}
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_R) {
			WUMMPlayer.NextImage();
		}
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_B) {
			WUMMPlayer.LoadFolder(WUMMPlayer.CurrentPath);
		}
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_Y) {
			WUMMPlayer.ResetImageScene();
		}
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_X) {
			if (WUMMPlayer.ImageMode == 'scale')
				WUMMPlayer.ImageMode = 'width';
			else if (WUMMPlayer.ImageMode == 'width')
				WUMMPlayer.ImageMode = 'scale';
			WUMMPlayer.ResetImageScene();
		}
	}
};

GamePad.JoystickLeftMove = function(event) {
    if (Math.abs(event.movementX) < 0.05) {
        event.movementX = 0;
    }
	if (Math.abs(event.movementY) < 0.05) {
        event.movementY = 0;
    }
};

GamePad.JoystickRightMove = function(event) {
    if (Math.abs(event.movementX) < 0.05) {
        event.movementX = 0;
    }
	if (Math.abs(event.movementY) < 0.05) {
        event.movementY = 0;
    }
	
	if (WUMMPlayer.Context == 'showing image' && WUMMPlayer.ImageMode == 'scale') {
		if (event.movementY < -0.95) {
			var scale = parseFloat(WUMMPlayer.ImageScale.toFixed(1)) + 0.1;
			WUMMPlayer.SetImageScale(scale);
			WUMMPlayer.CenterImage();
		}
		else if (event.movementY > 0.95) {
			var scale = parseFloat(WUMMPlayer.ImageScale.toFixed(1)) - 0.1;
			WUMMPlayer.SetImageScale(scale);
			WUMMPlayer.CenterImage();
		}
	}
};

GamePad.TouchStart = function(event) {

};

GamePad.TouchEnd = function(event) {

};

GamePad.TouchUpdate = function(event) {

};

GamePad.Vibrate = function(milliseconds) {
    if (GamePad.Connected()) {
        GamePad.Controller.startVibrate(milliseconds, 50, true);
    }
};

GamePad.VibrateStop = function() {
    if (GamePad.Connected()) {
        GamePad.Controller.stopVibrate();
    }
};


Wiimote.ButtonPress = function(event) {

};

Wiimote.ButtonRelease = function(event) {
	if (WUMMPlayer.Context == 'showing folder' || WUMMPlayer.Context == 'playing audio') {
		if (event.button == nwf.input.ControllerButton.WII_REMOTE_DOWN) {
			var numItems = 1 + WUMMPlayer.CurrentFolder.folders.length + WUMMPlayer.FileList.length;
			for (var i = 0; i < numItems; i++)
				document.getElementById('i' + i).className = 'item';
			WUMMPlayer.CurrentItemIndex++;
			if (WUMMPlayer.CurrentItemIndex >= numItems)
				WUMMPlayer.CurrentItemIndex = 0;
			document.getElementById('i' + WUMMPlayer.CurrentItemIndex).className = 'item selected';
		}
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_UP) {
			var numItems = 1 + WUMMPlayer.CurrentFolder.folders.length + WUMMPlayer.FileList.length;
			for (var i = 0; i < numItems; i++)
				document.getElementById('i' + i).className = 'item';
			WUMMPlayer.CurrentItemIndex--;
			if (WUMMPlayer.CurrentItemIndex < 0)
				WUMMPlayer.CurrentItemIndex = numItems - 1;
			document.getElementById('i' + WUMMPlayer.CurrentItemIndex).className = 'item selected';
		}
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_A && WUMMPlayer.CurrentItemIndex != -1) {
			if (WUMMPlayer.CurrentItemIndex == 0)
				WUMMPlayer.LoadFolder(WUMMPlayer.CurrentPath + '/..');
			else if (WUMMPlayer.CurrentFolder.folders.length > 0 &&
				WUMMPlayer.CurrentItemIndex - 1 < WUMMPlayer.CurrentFolder.folders.length)
				WUMMPlayer.LoadFolder(WUMMPlayer.CurrentPath + '/' + WUMMPlayer.CurrentFolder.folders[WUMMPlayer.CurrentItemIndex - 1].foldername);
			else if (WUMMPlayer.FileList.length > 0 &&
				WUMMPlayer.CurrentItemIndex - 1 - WUMMPlayer.CurrentFolder.folders.length < WUMMPlayer.FileList.length) {
				var index = WUMMPlayer.CurrentItemIndex - 1 - WUMMPlayer.CurrentFolder.folders.length;
				if (WUMMPlayer.IsVideoFile(WUMMPlayer.FileList[index]))
					WUMMPlayer.LoadVideo(index);
				else if (WUMMPlayer.IsAudioFile(WUMMPlayer.FileList[index]))
					WUMMPlayer.LoadAudio(index);
				else if (WUMMPlayer.IsImageFile(WUMMPlayer.FileList[index]))
					WUMMPlayer.LoadImage(index);
			}
		}
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_B) {
			if (WUMMPlayer.Context == 'showing folder')
				WUMMPlayer.LoadFolder(WUMMPlayer.CurrentPath + '/..');
			else if (WUMMPlayer.Context == 'playing audio')
				WUMMPlayer.LoadFolder(WUMMPlayer.CurrentPath);
		}
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_1 && WUMMPlayer.Context == 'playing audio') {
			WUMMPlayer.RandomAudioUpdate();
		}
	}
	else if (WUMMPlayer.Context == 'playing video') {
		if (event.button == nwf.input.ControllerButton.WII_REMOTE_B) {
			WUMMPlayer.LoadFolder(WUMMPlayer.CurrentPath);
		}
	}
	else if (WUMMPlayer.Context == 'showing image') {
		if (event.button == nwf.input.ControllerButton.WII_REMOTE_LEFT) {
			WUMMPlayer.PreviousImage();
		}
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_RIGHT) {
			WUMMPlayer.NextImage();
		}
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_B) {
			WUMMPlayer.LoadFolder(WUMMPlayer.CurrentPath);
		}
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_PLUS && WUMMPlayer.ImageMode == 'scale') {
			var scale = parseFloat(WUMMPlayer.ImageScale.toFixed(1)) + 0.1;
			WUMMPlayer.SetImageScale(scale);
			WUMMPlayer.CenterImage();
		}
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_MINUS && WUMMPlayer.ImageMode == 'scale') {
			var scale = parseFloat(WUMMPlayer.ImageScale.toFixed(1)) - 0.1;
			WUMMPlayer.SetImageScale(scale);
			WUMMPlayer.CenterImage();
		}
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_2) {
			WUMMPlayer.ResetImageScene();
		}
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_1) {
			if (WUMMPlayer.ImageMode == 'scale')
				WUMMPlayer.ImageMode = 'width';
			else if (WUMMPlayer.ImageMode == 'width')
				WUMMPlayer.ImageMode = 'scale';
			WUMMPlayer.ResetImageScene();
		}
	}
};

Wiimote.Vibrate = function(milliseconds) {
    if (Wiimote.Connected()) {
        Wiimote.Controller.startVibrate(milliseconds, 50, true);
    }
};

Wiimote.VibrateStop = function() {
    if (Wiimote.Connected()) {
        Wiimote.Controller.stopVibrate();
    }
};


Pro.ButtonPress = function(event) {
	if (WUMMPlayer.Context == 'showing image') {
		if (event.button == nwf.input.ControllerButton.PRO_UP) {
			var positionY = parseInt(document.getElementById("image").style.top) + 20;
			document.getElementById("image").style.top = positionY + 'px';
		}
		else if (event.button == nwf.input.ControllerButton.PRO_DOWN) {
			var positionY = parseInt(document.getElementById("image").style.top) - 20;
			document.getElementById("image").style.top = positionY + 'px';
		}
		if (event.button == nwf.input.ControllerButton.PRO_LEFT) {
			var positionX = parseInt(document.getElementById("image").style.right) + 20;
			document.getElementById("image").style.right = positionX + 'px';
		}
		else if (event.button == nwf.input.ControllerButton.PRO_RIGHT) {
			var positionX = parseInt(document.getElementById("image").style.right) - 20;
			document.getElementById("image").style.right = positionX + 'px';
		}
	}
};

Pro.ButtonRelease = function(event) {
	if (WUMMPlayer.Context == 'showing folder' || WUMMPlayer.Context == 'playing audio') {
		if (event.button == nwf.input.ControllerButton.PRO_DOWN) {
			var numItems = 1 + WUMMPlayer.CurrentFolder.folders.length + WUMMPlayer.FileList.length;
			for (var i = 0; i < numItems; i++)
				document.getElementById('i' + i).className = 'item';
			WUMMPlayer.CurrentItemIndex++;
			if (WUMMPlayer.CurrentItemIndex >= numItems)
				WUMMPlayer.CurrentItemIndex = 0;
			document.getElementById('i' + WUMMPlayer.CurrentItemIndex).className = 'item selected';
		}
		else if (event.button == nwf.input.ControllerButton.PRO_UP) {
			var numItems = 1 + WUMMPlayer.CurrentFolder.folders.length + WUMMPlayer.FileList.length;
			for (var i = 0; i < numItems; i++)
				document.getElementById('i' + i).className = 'item';
			WUMMPlayer.CurrentItemIndex--;
			if (WUMMPlayer.CurrentItemIndex < 0)
				WUMMPlayer.CurrentItemIndex = numItems - 1;
			document.getElementById('i' + WUMMPlayer.CurrentItemIndex).className = 'item selected';
		}
		else if (event.button == nwf.input.ControllerButton.PRO_A && WUMMPlayer.CurrentItemIndex != -1) {
			if (WUMMPlayer.CurrentItemIndex == 0)
				WUMMPlayer.LoadFolder(WUMMPlayer.CurrentPath + '/..');
			else if (WUMMPlayer.CurrentFolder.folders.length > 0 &&
				WUMMPlayer.CurrentItemIndex - 1 < WUMMPlayer.CurrentFolder.folders.length)
				WUMMPlayer.LoadFolder(WUMMPlayer.CurrentPath + '/' + WUMMPlayer.CurrentFolder.folders[WUMMPlayer.CurrentItemIndex - 1].foldername);
			else if (WUMMPlayer.FileList.length > 0 &&
				WUMMPlayer.CurrentItemIndex - 1 - WUMMPlayer.CurrentFolder.folders.length < WUMMPlayer.FileList.length) {
				var index = WUMMPlayer.CurrentItemIndex - 1 - WUMMPlayer.CurrentFolder.folders.length;
				if (WUMMPlayer.IsVideoFile(WUMMPlayer.FileList[index]))
					WUMMPlayer.LoadVideo(index);
				else if (WUMMPlayer.IsAudioFile(WUMMPlayer.FileList[index]))
					WUMMPlayer.LoadAudio(index);
				else if (WUMMPlayer.IsImageFile(WUMMPlayer.FileList[index]))
					WUMMPlayer.LoadImage(index);
			}
		}
		else if (event.button == nwf.input.ControllerButton.PRO_B) {
			if (WUMMPlayer.Context == 'showing folder')
				WUMMPlayer.LoadFolder(WUMMPlayer.CurrentPath + '/..');
			else if (WUMMPlayer.Context == 'playing audio')
				WUMMPlayer.LoadFolder(WUMMPlayer.CurrentPath);
		}
		else if (event.button == nwf.input.ControllerButton.PRO_X && WUMMPlayer.Context == 'playing audio') {
			WUMMPlayer.RandomAudioUpdate();
		}
	}
	else if (WUMMPlayer.Context == 'playing video') {
		if (event.button == nwf.input.ControllerButton.PRO_B) {
			WUMMPlayer.LoadFolder(WUMMPlayer.CurrentPath);
		}
	}
	else if (WUMMPlayer.Context == 'showing image') {
		if (event.button == nwf.input.ControllerButton.PRO_L) {
			WUMMPlayer.PreviousImage();
		}
		else if (event.button == nwf.input.ControllerButton.PRO_R) {
			WUMMPlayer.NextImage();
		}
		else if (event.button == nwf.input.ControllerButton.PRO_B) {
			WUMMPlayer.LoadFolder(WUMMPlayer.CurrentPath);
		}
		else if (event.button == nwf.input.ControllerButton.PRO_Y) {
			WUMMPlayer.ResetImageScene();
		}
		else if (event.button == nwf.input.ControllerButton.PRO_X) {
			if (WUMMPlayer.ImageMode == 'scale')
				WUMMPlayer.ImageMode = 'width';
			else if (WUMMPlayer.ImageMode == 'width')
				WUMMPlayer.ImageMode = 'scale';
			WUMMPlayer.ResetImageScene();
		}
	}
};

Pro.JoystickLeftMove = function(event) {
    if (Math.abs(event.movementX) < 0.05) {
        event.movementX = 0;
    }
	if (Math.abs(event.movementY) < 0.05) {
        event.movementY = 0;
    }
};

Pro.JoystickRightMove = function(event) {
    if (Math.abs(event.movementX) < 0.05) {
        event.movementX = 0;
    }
	if (Math.abs(event.movementY) < 0.05) {
        event.movementY = 0;
    }
	
	if (WUMMPlayer.Context == 'showing image' && WUMMPlayer.ImageMode == 'scale') {
		if (event.movementY < -0.95) {
			var scale = parseFloat(WUMMPlayer.ImageScale.toFixed(1)) + 0.1;
			WUMMPlayer.SetImageScale(scale);
			WUMMPlayer.CenterImage();
		}
		else if (event.movementY > 0.95) {
			var scale = parseFloat(WUMMPlayer.ImageScale.toFixed(1)) - 0.1;
			WUMMPlayer.SetImageScale(scale);
			WUMMPlayer.CenterImage();
		}
	}
};

Pro.Vibrate = function(milliseconds) {
    if (Pro.Connected()) {
        Pro.Controller.startVibrate(milliseconds, 50, true);
    }
};

Pro.VibrateStop = function() {
    if (Pro.Connected()) {
        Pro.Controller.stopVibrate();
    }
};