//WUM Player v1

WUMPlayer = {};

DevKeyboard = {};
GamePad = {};
Wiimote = {};
Pro = {};

GamePad.Controller = null;
Wiimote.Controller = null;
Pro.Controller = null;

WUMPlayer.Content = JSON.parse(fs);
WUMPlayer.IsWiiU = window.nwf && nwf.system && nwf.system.isWiiU();
WUMPlayer.Context = null;
WUMPlayer.ItemIndex = -1;
WUMPlayer.FileIndex = -1;
WUMPlayer.PathIndices = [0];
WUMPlayer.CurrentFolder = null;
WUMPlayer.CurrentPath = null;

WUMPlayer.MouseX = 0;
WUMPlayer.MouseY = 0;

WUMPlayer.ItemMarqueeId = null;
WUMPlayer.ItemListMouseOver = true;
WUMPlayer.ItemInner = null;
WUMPlayer.ItemHeight = 0;
WUMPlayer.ItemTextHeight = 0;
WUMPlayer.ItemIconWidth = 0;
WUMPlayer.ItemBorderWidth = 0;
WUMPlayer.ScrollbarWidth = 0;
WUMPlayer.PointRadius = 0;

WUMPlayer.RandomAudio = false;
WUMPlayer.CurrentAudio = null;
WUMPlayer.CurrentAudioDuration = null;
WUMPlayer.CurrentAudioDurationFormatted = null;
WUMPlayer.IsSeekingAudio = false;

WUMPlayer.ImageScale = 1.0;
WUMPlayer.ImageMode = 'scale';

WUMPlayer.ControllersInit = function() {
    if (WUMPlayer.IsWiiU) {
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

WUMPlayer.IsVideoFile = function(file) {
	return file.ext == '.mp4';
};

WUMPlayer.IsAudioFile = function(file) {
	return file.ext == '.m4a' || file.ext == '.ogg';
};

WUMPlayer.IsImageFile = function(file) {
	return file.ext == '.jpg' || file.ext == '.png' || file.ext == '.gif' || file.ext == '.bmp';
};


WUMPlayer.Init = function() {
	WUMPlayer.ControllersInit();
	WUMPlayer.SaveAutoDimming();
	WUMPlayer.SaveAutoPowerDown();
	WUMPlayer.SetCSS();

	WUMPlayer.CurrentFolder = WUMPlayer.Content;
	
	document.getElementById("content").setAttribute("onmousemove", "WUMPlayer.MouseMove(event)");
	
	if (WUMPlayer.CurrentFolder.folders.length == 0 &&
		WUMPlayer.CurrentFolder.files.length == 1 &&
		WUMPlayer.IsVideoFile(WUMPlayer.CurrentFolder.files[0])) {
		WUMPlayer.CurrentPath = WUMPlayer.CurrentFolder.foldername;
		WUMPlayer.LoadVideo(0);
	} else {
		WUMPlayer.LoadFolder(-2);
		if (WUMPlayer.CurrentFolder.folders.length > 0 ||
			WUMPlayer.CurrentFolder.files.length > 0) {
			document.getElementById('i0').className = 'item_selected';
			WUMPlayer.ItemIndex = 0;
			WUMPlayer.ChangeMarquee(document.getElementById('i0'));
		}
	}
};

WUMPlayer.SetCSS = function() {
	var windowWidth = window.innerWidth
	|| document.documentElement.clientWidth
	|| document.body.clientWidth;
	var windowHeight = window.innerHeight
	|| document.documentElement.clientHeight
	|| document.body.clientHeight;
	
	var css = document.createElement("link");
		css.setAttribute("rel", "stylesheet");
		css.setAttribute("type", "text/css");
		document.head.appendChild(css);
		
	if (windowWidth == 1920) {
		css.setAttribute("href", "wumplayer_style_1920.css");
		document.head.appendChild(css);
		WUMPlayer.ItemHeight = 141;
		WUMPlayer.ItemTextHeight = 110;
		WUMPlayer.ItemIconWidth = 123;
		WUMPlayer.ItemBorderWidth = 9;
		WUMPlayer.ScrollbarWidth = 125;
		WUMPlayer.PointRadius = 25;
	} else if (windowWidth == 1280) {
		css.setAttribute("href", "wumplayer_style_1280.css");
		document.head.appendChild(css);
		WUMPlayer.ItemHeight = 94;
		WUMPlayer.ItemTextHeight = 73;
		WUMPlayer.ItemIconWidth = 82;
		WUMPlayer.ItemBorderWidth = 6;
		WUMPlayer.ScrollbarWidth = 83;
		WUMPlayer.PointRadius = 17;
	} else if (windowWidth == 854) {
		css.setAttribute("href", "wumplayer_style_854.css");
		document.head.appendChild(css);
		WUMPlayer.ItemHeight = 63;
		WUMPlayer.ItemTextHeight = 49;
		WUMPlayer.ItemIconWidth = 55;
		WUMPlayer.ItemBorderWidth = 4;
		WUMPlayer.ScrollbarWidth = 56;
		WUMPlayer.PointRadius = 11;
	}
	else {
		css.setAttribute("href", "wumplayer_style_640.css");
		document.head.appendChild(css);
		WUMPlayer.ItemHeight = 63;
		WUMPlayer.ItemTextHeight = 49;
		WUMPlayer.ItemIconWidth = 55;
		WUMPlayer.ItemBorderWidth = 4;
		WUMPlayer.ScrollbarWidth = 56;
		WUMPlayer.PointRadius = 11;
	}
};

WUMPlayer.LoadFolder = function(index) {
	WUMPlayer.ResetAutoDimming();
	WUMPlayer.ResetAutoPowerDown();
	WUMPlayer.Context = 'showing folder';
	WUMPlayer.ItemIndex = -1;
	WUMPlayer.FileIndex = -1;

	if (index >= 0 && index < WUMPlayer.CurrentFolder.folders.length)
		WUMPlayer.PathIndices.push(index);
	else if (index == -1 && WUMPlayer.PathIndices.length > 1)
		WUMPlayer.PathIndices.pop(index);
	
	if (!WUMPlayer.IsWiiU) {
		document.body.setAttribute("onkeyup", "DevKeyboard.ChangeSelectedItem(event)");
		document.body.setAttribute("onkeydown", "");
	}
	
	var content = '<div id="debug" style="font-size: 32px; color: #00FF00; position: fixed; z-index: 3;"></div><div id="measure_text_helper" style="position: absolute;"></div><div id="mouse_point"></div><div id="back" onclick="WUMPlayer.LoadFolder(-1)"><img src="back.png"></div>';

	WUMPlayer.CurrentFolder = WUMPlayer.Content;
	var path = WUMPlayer.CurrentFolder.name;
	WUMPlayer.CurrentPath = WUMPlayer.CurrentFolder.foldername;
	for (var i = 1; i < WUMPlayer.PathIndices.length; i++) {
		WUMPlayer.CurrentFolder = WUMPlayer.CurrentFolder.folders[WUMPlayer.PathIndices[i]];
		path += '/' + WUMPlayer.CurrentFolder.name;
		WUMPlayer.CurrentPath += '/' + WUMPlayer.CurrentFolder.foldername;
	}

	content += '<div id="info"><span id="path"><div>' + path + '</div></span></div><div id="corner"></div>';
	
	var list = '<div id="item_list">';
	for (var i = 0; i < WUMPlayer.CurrentFolder.folders.length; i++) {
		list += '<div id="i' + i + '" class="item" onclick="WUMPlayer.LoadFolder(' + i + ')" onmouseover="WUMPlayer.ItemMouseOver(this)" onmouseout="WUMPlayer.ItemMouseOut(this)"><div class="marquee"><img src="folder.png"><span>' + WUMPlayer.CurrentFolder.folders[i].name + '</span></div></div>';
	}
	for (var i = 0; i < WUMPlayer.CurrentFolder.files.length; i++) {
		list += '<div id="i' + (i + WUMPlayer.CurrentFolder.folders.length) + '" class="item" onclick="WUMPlayer.LoadFile(' + i + ')" onmouseover="WUMPlayer.ItemMouseOver(this)" onmouseout="WUMPlayer.ItemMouseOut(this)"><div class="marquee">';
		if (WUMPlayer.IsVideoFile(WUMPlayer.CurrentFolder.files[i]))
			list += '<img src="video.png">';
		else if (WUMPlayer.IsAudioFile(WUMPlayer.CurrentFolder.files[i]))
			list += '<img src="audio.png">';
		else if (WUMPlayer.IsImageFile(WUMPlayer.CurrentFolder.files[i]))
			list += '<img src="image.png">';
		else
			list += '<img src="unknown.png">';
		list += '<span>' + WUMPlayer.CurrentFolder.files[i].name + '</span></div></div>';
	}
	list += '</div>';

	content += '<div id="list">' + list + '</div>';

	document.getElementById('content').innerHTML = content;
	
	var point = document.getElementById("mouse_point");
	point.style.left = (WUMPlayer.MouseX - WUMPlayer.PointRadius) + "px";
	point.style.top = (WUMPlayer.MouseY - WUMPlayer.PointRadius) + "px";
};

WUMPlayer.PreviousItem = function() {
	var numItems = WUMPlayer.CurrentFolder.folders.length + WUMPlayer.CurrentFolder.files.length;
		
	WUMPlayer.ItemIndex--;
	if (WUMPlayer.ItemIndex < 0)
		WUMPlayer.ItemIndex = numItems - 1;
	
	var itemSelected = document.getElementsByClassName("item_selected")[0];
	if (itemSelected != null) {
		itemSelected.getElementsByTagName("div")[0].style.left = WUMPlayer.ItemBorderWidth + "px";
		if (WUMPlayer.ItemInner != null) {
			itemSelected.getElementsByTagName("div")[0].innerHTML = WUMPlayer.ItemInner;
		}
		itemSelected.className = 'item';
	}
	
	var element = document.getElementById('i' + WUMPlayer.ItemIndex);
	element.className = 'item_selected';
	
	var list = document.getElementById("list");
	var y1 = list.scrollTop;
	var y2 = list.scrollTop + list.clientHeight - WUMPlayer.ItemTextHeight;
	
	if (WUMPlayer.ItemIndex * WUMPlayer.ItemHeight < y1) {
		list.scrollTop = WUMPlayer.ItemIndex * WUMPlayer.ItemHeight;
	} else if (WUMPlayer.ItemIndex == numItems - 1) {
		list.scrollTop = numItems * WUMPlayer.ItemHeight - (list.clientHeight - WUMPlayer.ItemTextHeight);
	}

	WUMPlayer.ChangeMarquee(element);
};

WUMPlayer.NextItem = function() {
	//document.getElementById('debug').innerHTML = WUMPlayer.ItemIndex;
	var numItems = WUMPlayer.CurrentFolder.folders.length + WUMPlayer.CurrentFolder.files.length;
		
	WUMPlayer.ItemIndex++;
	if (WUMPlayer.ItemIndex >= numItems)
		WUMPlayer.ItemIndex = 0;

	var itemSelected = document.getElementsByClassName("item_selected")[0];
	if (itemSelected != null) {
		itemSelected.getElementsByTagName("div")[0].style.left = WUMPlayer.ItemBorderWidth + "px";
		if (WUMPlayer.ItemInner != null) {
			itemSelected.getElementsByTagName("div")[0].innerHTML = WUMPlayer.ItemInner;
		}
		itemSelected.className = 'item';
	}

	var element = document.getElementById('i' + WUMPlayer.ItemIndex);
	element.className = 'item_selected';

	var list = document.getElementById("list");
	var y1 = list.scrollTop;
	var y2 = list.scrollTop + list.clientHeight - WUMPlayer.ItemTextHeight;

	if (WUMPlayer.ItemHeight * (WUMPlayer.ItemIndex + 1) > y2) {
		list.scrollTop = WUMPlayer.ItemHeight * (WUMPlayer.ItemIndex + 1) - (list.clientHeight - WUMPlayer.ItemTextHeight);
	} else if (WUMPlayer.ItemIndex * WUMPlayer.ItemHeight < y1) {
		list.scrollTop = WUMPlayer.ItemIndex * WUMPlayer.ItemHeight;
	}
	
	WUMPlayer.ChangeMarquee(element);
};

WUMPlayer.SelectItem = function() {
	if (WUMPlayer.ItemIndex < WUMPlayer.CurrentFolder.folders.length)
		WUMPlayer.LoadFolder(WUMPlayer.ItemIndex);
	else {
		var index = WUMPlayer.ItemIndex - WUMPlayer.CurrentFolder.folders.length;
		WUMPlayer.LoadFile(index);
	}
};

WUMPlayer.Back = function() {
	if (WUMPlayer.Context == 'showing folder')
		WUMPlayer.LoadFolder(-1);
	else if (WUMPlayer.Context == 'playing audio' ||
		WUMPlayer.Context == 'playing video' ||
		WUMPlayer.Context == 'showing image')
		WUMPlayer.LoadFolder(-2);
};

WUMPlayer.ChangeMarquee = function(element) {
	clearInterval(WUMPlayer.ItemMarqueeId);

	var windowWidth = window.innerWidth
	|| document.documentElement.clientWidth
	|| document.body.clientWidth;
	
	var text = element.getElementsByTagName("div")[0].getElementsByTagName("span")[0].innerHTML;
	
	var textWidth = WUMPlayer.GetTextWidth(text, WUMPlayer.ItemTextHeight);

	element.getElementsByTagName("div")[0].style.left = "0px";
	WUMPlayer.ItemInner = element.getElementsByTagName("div")[0].innerHTML;

	if (textWidth + WUMPlayer.ItemIconWidth + WUMPlayer.ItemBorderWidth > windowWidth - WUMPlayer.ScrollbarWidth) {
		element.getElementsByTagName("div")[0].innerHTML = WUMPlayer.ItemInner + WUMPlayer.ItemInner;
		var position = 0;
		WUMPlayer.ItemMarqueeId = setInterval(marquee, 5);
		function marquee() {
			if (position == -(textWidth + WUMPlayer.ItemIconWidth))
				position = 0;
			position--; 
			element.getElementsByTagName("div")[0].style.left = position + "px"; 
		}
	}
};

WUMPlayer.GetTextWidth = function(text, fontSize) {
	var measureTextHelper = document.getElementById("measure_text_helper");
	measureTextHelper.innerHTML = text;
	measureTextHelper.style.fontSize = fontSize;
	return measureTextHelper.clientWidth + 1;
};


WUMPlayer.LoadFile = function(index) {
	if (WUMPlayer.IsVideoFile(WUMPlayer.CurrentFolder.files[index]))
		WUMPlayer.LoadVideo(index);
	else if (WUMPlayer.IsAudioFile(WUMPlayer.CurrentFolder.files[index]))
		WUMPlayer.LoadAudio(index);
	else if (WUMPlayer.IsImageFile(WUMPlayer.CurrentFolder.files[index]))
		WUMPlayer.LoadImage(index);
	else
		WUMPlayer.LoadUnknown(index);
};


WUMPlayer.LoadAudio = function(index) {
	WUMPlayer.Context = 'playing audio';
	
	if (WUMPlayer.FileIndex != -1)
		document.getElementById('i' + WUMPlayer.FileIndex).style.backgroundColor = "transparent";
	WUMPlayer.FileIndex = index;
	
	document.getElementById("back").setAttribute("onclick", "WUMPlayer.LoadFolder(-2)");
	if (!WUMPlayer.IsWiiU) {
		document.body.setAttribute("onkeyup", "DevKeyboard.ChangeSelectedItem(event)");
		document.body.setAttribute("onkeydown", "");
	}
	
	WUMPlayer.DisableAutoDimming();
	WUMPlayer.DisableAutoPowerDown();
	
	var i = WUMPlayer.CurrentFolder.folders.length + index;
	document.getElementById('i' + i).style.backgroundColor = "rgba(0, 28, 78, 1)";
	
	var audio = '<audio id="audio" onloadedmetadata="WUMPlayer.UpdateAudioSlider()" ontimeupdate="WUMPlayer.TimeUpdateAudio()" onended="WUMPlayer.NextAudio()"><source src="';
	if (WUMPlayer.CurrentFolder.files[index].ext == '.m4a')
		audio += WUMPlayer.CurrentPath + '/' + WUMPlayer.CurrentFolder.files[index].filename + WUMPlayer.CurrentFolder.files[index].ext + '" type="audio/aac"></audio>';
	else if (WUMPlayer.CurrentFolder.files[index].ext == '.ogg')
		audio += WUMPlayer.CurrentPath + '/' + WUMPlayer.CurrentFolder.files[index].filename + WUMPlayer.CurrentFolder.files[index].ext + '" type="audio/ogg"></audio>';
	
	audio += '<buttom id="play_audio" onclick="WUMPlayer.PlayPauseAudio()"><img id="play_audio_img" src="play_audio.png"></buttom><buttom id="next_audio" onclick="WUMPlayer.NextAudio()"><img id="next_audio_img" src="next_audio.png"></buttom><div id="time"></div><input type="range" id="audio_slider" onmousedown="WUMPlayer.SeekingAudio()" onmouseup="WUMPlayer.SeekedAudio()" oninput="WUMPlayer.UpdateTimeFormatAudio()">';
	
	if (WUMPlayer.RandomAudio)
		audio += '<buttom id="random_audio" onclick="WUMPlayer.ChangeRandomAudio()"><img id="random_audio_img" src="random_audio_blue.png"></buttom>';
	else
		audio += '<buttom id="random_audio" onclick="WUMPlayer.ChangeRandomAudio()"><img id="random_audio_img" src="random_audio.png"></buttom>';
	
	document.getElementById('info').innerHTML = audio;
};

WUMPlayer.UpdateAudioSlider = function() {
	WUMPlayer.CurrentAudio = document.getElementById('audio'); 
	WUMPlayer.CurrentAudioDuration = (WUMPlayer.CurrentAudio.duration * 1000).toFixed(0);
	WUMPlayer.CurrentAudioDurationFormatted = WUMPlayer.GetTimeFormat(WUMPlayer.CurrentAudioDuration);

	var slider = document.getElementById('audio_slider');
	slider.min = 0; 
	slider.max = WUMPlayer.CurrentAudioDuration; 
	slider.value = 0;
	
	WUMPlayer.UpdateTimeFormatAudio();
	WUMPlayer.PlayAudio();
};

WUMPlayer.ChangeRandomAudio = function() {
	WUMPlayer.RandomAudio = !WUMPlayer.RandomAudio;
	if (WUMPlayer.RandomAudio)
		document.getElementById('random_audio').innerHTML = '<img id="random_audio_img" src="random_audio_blue.png">';
	else 
		document.getElementById('random_audio').innerHTML = '<img id="random_audio_img" src="random_audio.png">';
};

WUMPlayer.NextAudio = function() {
	var i;
	if (WUMPlayer.RandomAudio) {
		i = Math.floor(Math.random() * WUMPlayer.CurrentFolder.files.length);
	}
	else {
		i = WUMPlayer.FileIndex + 1;
		if (i == WUMPlayer.CurrentFolder.files.length)
			i = 0;
	}
	while (!WUMPlayer.IsAudioFile(WUMPlayer.CurrentFolder.files[i])) {
		i++;
		if (i == WUMPlayer.CurrentFolder.files.length)
			i = 0;
	}
	WUMPlayer.LoadAudio(i);
};


WUMPlayer.PlayPauseAudio = function() { 
	if (WUMPlayer.CurrentAudio.paused)
		WUMPlayer.PlayAudio();
	else
		WUMPlayer.PauseAudio(); 
};

WUMPlayer.PlayAudio = function() {
	document.getElementById('play_audio').innerHTML = '<img id="play_audio_img" src="pause_audio.png">'; 
	WUMPlayer.CurrentAudio.play();
};

WUMPlayer.PauseAudio = function() { 
	document.getElementById('play_audio').innerHTML = '<img id="play_audio_img" src="play_audio.png">'; 
	WUMPlayer.CurrentAudio.pause(); 
};

WUMPlayer.TimeUpdateAudio = function() {
	if (WUMPlayer.Context == 'playing audio' && !WUMPlayer.IsSeekingAudio) {
		document.getElementById('audio_slider').value = WUMPlayer.CurrentAudio.currentTime * 1000;
		WUMPlayer.UpdateTimeFormatAudio();
	}
};

WUMPlayer.SeekingAudio = function() {
    WUMPlayer.IsSeekingAudio = true;
};

WUMPlayer.SeekedAudio = function() {
    WUMPlayer.CurrentAudio.currentTime = document.getElementById('audio_slider').value / 1000.0;
	WUMPlayer.UpdateTimeFormatAudio();
	WUMPlayer.IsSeekingAudio = false;
};

WUMPlayer.UpdateTimeFormatAudio = function() {
	document.getElementById('time').innerHTML = 
		WUMPlayer.GetTimeFormat(document.getElementById('audio_slider').value) +
		' / ' + WUMPlayer.CurrentAudioDurationFormatted;
};

WUMPlayer.GetTimeFormat = function(milliseconds) {
	var seconds = parseInt(milliseconds / 1000.0);
	var minutes = parseInt(seconds / 60.0);
	var hours = parseInt(minutes / 60.0);
	seconds -= minutes * 60;
	minutes -= hours * 60;

	if (WUMPlayer.CurrentAudioDuration >= 3600000)
		return hours + ':' + (minutes < 10 ? '0' + minutes : '' + minutes) + ':' +
		(seconds < 10 ? '0' + seconds : '' + seconds);
	else
		return minutes + ':' + (seconds < 10 ? '0' + seconds : '' + seconds);
};


WUMPlayer.LoadVideo = function(index) {
	WUMPlayer.Context = 'playing video';
	WUMPlayer.FileIndex = index;
		
	if (!WUMPlayer.IsWiiU) {
		document.body.setAttribute("onkeyup", "DevKeyboard.VideoKeyUp(event)");
		document.body.setAttribute("onkeydown", "");
	}
		
	WUMPlayer.DisableAutoDimming();
	WUMPlayer.DisableAutoPowerDown();
	
	var content = '<div id="mouse_point"></div><div id="back" onclick="WUMPlayer.LoadFolder(-2)"><img src="back.png"></div><div id="info"><span id="name"><div>' + WUMPlayer.CurrentFolder.files[index].name + '</div></span></div><div id="corner"></div><video id="video" onended="WUMPlayer.NextVideo()" controls autoplay><source src="' + WUMPlayer.CurrentPath + '/' + WUMPlayer.CurrentFolder.files[index].filename + WUMPlayer.CurrentFolder.files[index].ext + '" type="video/mp4">Your browser does not support the video tag.</video>';
	document.getElementById('content').innerHTML = content;
	
	var point = document.getElementById("mouse_point");
	point.style.left = (WUMPlayer.MouseX - WUMPlayer.PointRadius) + "px";
	point.style.top = (WUMPlayer.MouseY - WUMPlayer.PointRadius) + "px";
};

WUMPlayer.NextVideo = function() {
	var i = WUMPlayer.FileIndex + 1;
	if (i == WUMPlayer.CurrentFolder.files.length)
		i = 0;
	while (!WUMPlayer.IsVideoFile(WUMPlayer.CurrentFolder.files[i])) {
		i++;
		if (i == WUMPlayer.CurrentFolder.files.length)
			i = 0;
	}
	WUMPlayer.LoadVideo(i);
};


WUMPlayer.LoadImage = function(index) {
	WUMPlayer.Context = 'showing image';
	WUMPlayer.FileIndex = index;
	
	if (!WUMPlayer.IsWiiU) {
		document.body.setAttribute("onkeyup", "DevKeyboard.ImageKeyUp(event)");
		document.body.setAttribute("onkeydown", "DevKeyboard.ImageKeyDown(event)");
	}
	
	var content = '<div id="mouse_point"></div><div id="back_image" onclick="WUMPlayer.LoadFolder(-2)"><img src="back.png"></div><div id="info_image"><span id="name"><div>' + WUMPlayer.CurrentFolder.files[index].name + '</div></span></div><div id="image_bg" onmousemove="WUMPlayer.ShowUIImage()"><img id="image" src="' + WUMPlayer.CurrentPath + '/' + WUMPlayer.CurrentFolder.files[index].filename + WUMPlayer.CurrentFolder.files[index].ext + '" /></div>';
	document.getElementById('content').innerHTML = content;

	WUMPlayer.ResetImageScene();
	
	var point = document.getElementById("mouse_point");
	point.style.left = (WUMPlayer.MouseX - WUMPlayer.PointRadius) + "px";
	point.style.top = (WUMPlayer.MouseY - WUMPlayer.PointRadius) + "px";
};

WUMPlayer.ShowUIImage = function() {
	var name = document.getElementById('info_image');
	var backImage = document.getElementById('back_image');
	
	name.style.webkitAnimationName = 'none';
	setTimeout(function() {
        name.style.webkitAnimationName = '';
    }, 10);
	
	backImage.style.webkitAnimationName = 'none';
	setTimeout(function() {
        backImage.style.webkitAnimationName = '';
    }, 10);
};

WUMPlayer.ResetImageScene = function() {
	window.scrollTo(0, 0);
	var windowWidth = window.innerWidth
	|| document.documentElement.clientWidth
	|| document.body.clientWidth;
	var windowHeight = window.innerHeight
	|| document.documentElement.clientHeight
	|| document.body.clientHeight;
	
	document.getElementById("image_bg").style.width = windowWidth + 'px';
	document.getElementById("image_bg").style.height = windowHeight + 'px';

	WUMPlayer.SetImageScale(1);
	
	if (WUMPlayer.ImageMode == 'scale') {
		var widthScale = windowWidth / WUMPlayer.CurrentFolder.files[WUMPlayer.FileIndex].width;
		var heightScale = windowHeight / WUMPlayer.CurrentFolder.files[WUMPlayer.FileIndex].height;
		if (widthScale < 1 || heightScale < 1) {
			if (widthScale < heightScale)
				WUMPlayer.SetImageScale(widthScale);
			else
				WUMPlayer.SetImageScale(heightScale);
		}
		WUMPlayer.CenterImage();
	}
	else if (WUMPlayer.ImageMode == 'width') {
		var scale = windowWidth / WUMPlayer.CurrentFolder.files[WUMPlayer.FileIndex].width;
		WUMPlayer.SetImageScale(scale);
		document.getElementById("image").style.right = '0px';
		document.getElementById("image").style.top = '0px';
	}
};

WUMPlayer.SetImageScale = function(scale) {
	if (scale >= 0.1 && scale <= 10.0) {
		WUMPlayer.ImageScale = scale;
		var width = parseInt(WUMPlayer.CurrentFolder.files[WUMPlayer.FileIndex].width * scale);
		var height = parseInt(WUMPlayer.CurrentFolder.files[WUMPlayer.FileIndex].height * scale);
		document.getElementById("image").style.width = width + 'px';
		document.getElementById("image").style.height = height + 'px';
	}
};

WUMPlayer.CenterImage = function() {
	var windowWidth = window.innerWidth
	|| document.documentElement.clientWidth
	|| document.body.clientWidth;
	var windowHeight = window.innerHeight
	|| document.documentElement.clientHeight
	|| document.body.clientHeight;
	
	var image = document.getElementById("image");
	var imageWidth = parseInt(image.style.width);
	var imageHeight = parseInt(image.style.height);
	var x = 0;
	var y = 0;
	image.style.left = '0px';
	image.style.top = '0px';
	
	if (imageWidth <= windowWidth)
		image.style.left = Math.floor((windowWidth - imageWidth) / 2) + 'px';
	else
		x = Math.floor((imageWidth - windowWidth) / 2);
	if (imageHeight <= windowHeight)
		image.style.top = Math.floor((windowHeight - imageHeight) / 2) + 'px';
	else
		y = Math.floor((imageHeight - windowHeight) / 2);
	
	document.getElementById("image_bg").scrollLeft = x;
	document.getElementById("image_bg").scrollTop = y;
};

WUMPlayer.PreviousImage = function() {
	var i = WUMPlayer.FileIndex - 1;
	if (i == -1)
		i = WUMPlayer.CurrentFolder.files.length - 1;
	while (!WUMPlayer.IsImageFile(WUMPlayer.CurrentFolder.files[i])) {
		i--;
		if (i == -1)
			i = WUMPlayer.CurrentFolder.files.length - 1;
	}
	WUMPlayer.LoadImage(i);
};

WUMPlayer.NextImage = function() {
	var i = WUMPlayer.FileIndex + 1;
	if (i == WUMPlayer.CurrentFolder.files.length)
		i = 0;
	while (!WUMPlayer.IsImageFile(WUMPlayer.CurrentFolder.files[i])) {
		i++;
		if (i == WUMPlayer.CurrentFolder.files.length)
			i = 0;
	}
	WUMPlayer.LoadImage(i);
};

WUMPlayer.ChangeImageMode = function() {
	if (WUMPlayer.ImageMode == 'scale')
		WUMPlayer.ImageMode = 'width';
	else if (WUMPlayer.ImageMode == 'width')
		WUMPlayer.ImageMode = 'scale';
	WUMPlayer.ResetImageScene();
};

WUMPlayer.IncreaseImageScale = function() {
	var scale = parseFloat(WUMPlayer.ImageScale.toFixed(1)) + 0.1;
	WUMPlayer.SetImageScale(scale);
	WUMPlayer.CenterImage();
};

WUMPlayer.DecreaseImageScale = function() {
	var scale = parseFloat(WUMPlayer.ImageScale.toFixed(1)) - 0.1;
	WUMPlayer.SetImageScale(scale);
	WUMPlayer.CenterImage();
};

WUMPlayer.MoveImageUp = function() {
	var bg = document.getElementById("image_bg");
	if (bg.scrollTop > 20)
		bg.scrollTop -= 20;
	else
		bg.scrollTop = 0;
};

WUMPlayer.MoveImageDown = function() {
	var bg = document.getElementById("image_bg");
	if (bg.scrollTop < bg.scrollHeight - 20)
		bg.scrollTop += 20;
	else
		bg.scrollTop = bg.scrollHeight;
};

WUMPlayer.MoveImageLeft = function() {
	var bg = document.getElementById("image_bg");
	if (bg.scrollLeft > 20)
		bg.scrollLeft -= 20;
	else
		bg.scrollLeft = 0;
};

WUMPlayer.MoveImageRight = function() {
	var bg = document.getElementById("image_bg");
	if (bg.scrollLeft < bg.scrollWidth - 20)
		bg.scrollLeft += 20;
	else
		bg.scrollLeft = bg.scrollWidth;
};


WUMPlayer.SaveAutoDimming = function(){
    if (WUMPlayer.IsWiiU) { 
		WUMPlayer.DimmingTV = nwf.display.DisplayManager.getInstance().getTVDisplay().dimmingEnabled;
		WUMPlayer.DimmingGamePad = nwf.display.DisplayManager.getInstance().getGamePadDisplay().dimmingEnabled;
    }
};

WUMPlayer.ResetAutoDimming = function() {
    if (WUMPlayer.IsWiiU) {      
		nwf.display.DisplayManager.getInstance().getTVDisplay().dimmingEnabled = WUMPlayer.DimmingTV;
		nwf.display.DisplayManager.getInstance().getGamePadDisplay().dimmingEnabled = WUMPlayer.DimmingGamePad;
    }
};
  
WUMPlayer.DisableAutoDimming = function() {
    if (WUMPlayer.IsWiiU) {      
		nwf.display.DisplayManager.getInstance().getTVDisplay().dimmingEnabled = false;
		nwf.display.DisplayManager.getInstance().getGamePadDisplay().dimmingEnabled = false;
    }
};

WUMPlayer.SaveAutoPowerDown = function() {
	if (WUMPlayer.IsWiiU) {
		WUMPlayer.AutoPowerDown = nwf.system.APD.isEnabled;
	}
};

WUMPlayer.ResetAutoPowerDown = function() {
	if (WUMPlayer.IsWiiU) {
		if (WUMPlayer.AutoPowerDown) {
			try {
				nwf.system.APD.enable();
			} catch (ex) {
				
			}
		}
	}
};

WUMPlayer.DisableAutoPowerDown = function() {
	if (WUMPlayer.IsWiiU && nwf.system.APD.isEnabled) {
		try {
			nwf.system.APD.disable();
		} catch(ex) {

		}
	}
};


WUMPlayer.MouseMove = function(event) {
	var point = document.getElementById("mouse_point");
	
	point.style.left = (event.clientX - WUMPlayer.PointRadius) + "px";
	point.style.top = (event.clientY - WUMPlayer.PointRadius) + "px";
	
	WUMPlayer.MouseX = event.clientX;
	WUMPlayer.MouseY = event.clientY;
}

WUMPlayer.ItemMouseOver = function(element) {
	WUMPlayer.ItemListMouseOver = true;
	
	var itemSelected = document.getElementsByClassName("item_selected")[0];
	if (itemSelected != null) {
		itemSelected.getElementsByTagName("div")[0].style.left = WUMPlayer.ItemBorderWidth + "px";
		if (WUMPlayer.ItemInner != null)
			itemSelected.getElementsByTagName("div")[0].innerHTML = WUMPlayer.ItemInner;
		itemSelected.className = 'item';
	}

	element.className = 'item_selected';
	WUMPlayer.ItemIndex = parseInt(element.id.slice(1));

	WUMPlayer.ChangeMarquee(element);
};

WUMPlayer.ItemMouseOut = function(element) {
	WUMPlayer.ItemListMouseOver = false;
};


DevKeyboard.ChangeSelectedItem = function(event) {
	if (event.key == 'ArrowDown' && !WUMPlayer.ItemListMouseOver)
		WUMPlayer.NextItem();
	else if (event.key == 'ArrowUp' && !WUMPlayer.ItemListMouseOver)
		WUMPlayer.PreviousItem();
	else if (event.key == 'x' && WUMPlayer.ItemIndex != -1)
		WUMPlayer.SelectItem();
	else if (event.key == 'z')
		WUMPlayer.Back();
	else if (event.key == 'c' && WUMPlayer.Context == 'playing audio')
		WUMPlayer.ChangeRandomAudio();
	else if (event.key == 'v' && WUMPlayer.Context == 'playing audio')
		WUMPlayer.PlayPauseAudio();
	else if (event.key == 's' && WUMPlayer.Context == 'playing audio')
		WUMPlayer.NextAudio();
};

DevKeyboard.VideoKeyUp = function(event) {
	if (event.key == 'z') {
		WUMPlayer.Back();
	}
};

DevKeyboard.ImageKeyUp = function(event) {
	if (event.key == 'a')
		WUMPlayer.PreviousImage();
	else if (event.key == 's')
		WUMPlayer.NextImage();
	else if (event.key == 'z')
		WUMPlayer.LoadFolder(-2);
	else if (event.key == 'v')
		WUMPlayer.ResetImageScene();
	else if (event.key == 'c')
		WUMPlayer.ChangeImageMode();
	else if (event.key == 'h' && WUMPlayer.ImageMode == 'scale')
		WUMPlayer.IncreaseImageScale();
	else if (event.key == 'n' && WUMPlayer.ImageMode == 'scale')
		WUMPlayer.DecreaseImageScale();
};

DevKeyboard.ImageKeyDown = function(event) {
	if (event.key == 'k')
		WUMPlayer.MoveImageUp();
	else if (event.key == 'i')
		WUMPlayer.MoveImageDown();
	else if (event.key == 'j')
		WUMPlayer.MoveImageLeft();
	else if (event.key == 'l')
		WUMPlayer.MoveImageRight();
};


GamePad.Connected = function() {
	return WUMPlayer.IsWiiU && GamePad.Controller && GamePad.Controller.connected;
};

Wiimote.Connected = function() {
	return WUMPlayer.IsWiiU && Wiimote.Controller && Wiimote.Controller.connected;
};

Pro.Connected = function() {
	return WUMPlayer.IsWiiU && Pro.Controller && Pro.Controller.connected;
};


GamePad.ButtonPress = function(event) {
	if (WUMPlayer.Context == 'showing image') {
		if (event.button == nwf.input.ControllerButton.GAMEPAD_UP) 
			WUMPlayer.MoveImageUp();
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_DOWN) 
			WUMPlayer.MoveImageDown();
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_LEFT) 
			WUMPlayer.MoveImageLeft();
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_RIGHT) 
			WUMPlayer.MoveImageRight();
	}
};

GamePad.ButtonRelease = function(event) {
	if (WUMPlayer.Context == 'showing folder' || WUMPlayer.Context == 'playing audio') {
		if (event.button == nwf.input.ControllerButton.GAMEPAD_DOWN && !WUMPlayer.ItemListMouseOver)
			WUMPlayer.NextItem();
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_UP && !WUMPlayer.ItemListMouseOver)
			WUMPlayer.PreviousItem();
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_A && WUMPlayer.ItemIndex != -1)
			WUMPlayer.SelectItem();
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_B)
			WUMPlayer.Back();
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_X && WUMPlayer.Context == 'playing audio')
			WUMPlayer.ChangeRandomAudio();
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_Y && WUMPlayer.Context == 'playing audio')
			WUMPlayer.PlayPauseAudio();
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_R && WUMPlayer.Context == 'playing audio')
			WUMPlayer.NextAudio();
	}
	else if (WUMPlayer.Context == 'playing video') {
		if (event.button == nwf.input.ControllerButton.GAMEPAD_B)
			WUMPlayer.Back();
	}
	else if (WUMPlayer.Context == 'showing image') {
		if (event.button == nwf.input.ControllerButton.GAMEPAD_L)
			WUMPlayer.PreviousImage();
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_R)
			WUMPlayer.NextImage();
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_B)
			WUMPlayer.Back();
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_Y)
			WUMPlayer.ResetImageScene();
		else if (event.button == nwf.input.ControllerButton.GAMEPAD_X)
			WUMPlayer.ChangeImageMode();
	}
};

GamePad.JoystickLeftMove = function(event) {
    if (Math.abs(event.movementX) < 0.05) {
        event.movementX = 0;
    }
	if (Math.abs(event.movementY) < 0.05) {
        event.movementY = 0;
    }

	if (WUMPlayer.Context == 'showing image') {
		var bg = document.getElementById("image_bg");
		
		if (event.movementX < -0.80 && bg.scrollLeft > 0)
			WUMPlayer.MoveImageLeft();
		else if (event.movementX > 0.80 && bg.scrollLeft < bg.scrollWidth)
			WUMPlayer.MoveImageRight();
		
		if (event.movementY < -0.80 && bg.scrollTop > 0)
			WUMPlayer.MoveImageUp();
		else if (event.movementY > 0.80 && bg.scrollTop < bg.scrollHeight)
			WUMPlayer.MoveImageDown();
	}
};

GamePad.JoystickRightMove = function(event) {
    if (Math.abs(event.movementX) < 0.05) {
        event.movementX = 0;
    }
	if (Math.abs(event.movementY) < 0.05) {
        event.movementY = 0;
    }
	
	if (WUMPlayer.Context == 'showing image' && WUMPlayer.ImageMode == 'scale') {
		if (event.movementY < -0.80)
			WUMPlayer.IncreaseImageScale();
		else if (event.movementY > 0.80)
			WUMPlayer.DecreaseImageScale();
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
	if (WUMPlayer.Context == 'showing folder' || WUMPlayer.Context == 'playing audio') {
		if (event.button == nwf.input.ControllerButton.WII_REMOTE_DOWN && !WUMPlayer.ItemListMouseOver)
			WUMPlayer.NextItem();
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_UP && !WUMPlayer.ItemListMouseOver)
			WUMPlayer.PreviousItem();
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_A && WUMPlayer.ItemIndex != -1)
			WUMPlayer.SelectItem();
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_B)
			WUMPlayer.Back();
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_1 && WUMPlayer.Context == 'playing audio')
			WUMPlayer.ChangeRandomAudio();
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_2 && WUMPlayer.Context == 'playing audio')
			WUMPlayer.PlayPauseAudio();
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_PLUS && WUMPlayer.Context == 'playing audio')
			WUMPlayer.NextAudio();
	}
	else if (WUMPlayer.Context == 'playing video') {
		if (event.button == nwf.input.ControllerButton.WII_REMOTE_B)
			WUMPlayer.Back();
	}
	else if (WUMPlayer.Context == 'showing image') {
		if (event.button == nwf.input.ControllerButton.WII_REMOTE_LEFT)
			WUMPlayer.PreviousImage();
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_RIGHT)
			WUMPlayer.NextImage();
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_B)
			WUMPlayer.Back();
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_PLUS && WUMPlayer.ImageMode == 'scale')
			WUMPlayer.IncreaseImageScale();
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_MINUS && WUMPlayer.ImageMode == 'scale')
			WUMPlayer.DecreaseImageScale();
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_2)
			WUMPlayer.ResetImageScene();
		else if (event.button == nwf.input.ControllerButton.WII_REMOTE_1)
			WUMPlayer.ChangeImageMode();
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
	if (WUMPlayer.Context == 'showing image') {
		if (event.button == nwf.input.ControllerButton.PRO_UP)
			WUMPlayer.MoveImageUp();
		else if (event.button == nwf.input.ControllerButton.PRO_DOWN)
			WUMPlayer.MoveImageDown();
		else if (event.button == nwf.input.ControllerButton.PRO_LEFT)
			WUMPlayer.MoveImageLeft();
		else if (event.button == nwf.input.ControllerButton.PRO_RIGHT)
			WUMPlayer.MoveImageRight();
	}
};

Pro.ButtonRelease = function(event) {
	if (WUMPlayer.Context == 'showing folder' || WUMPlayer.Context == 'playing audio') {
		if (event.button == nwf.input.ControllerButton.PRO_DOWN && !WUMPlayer.ItemListMouseOver)
			WUMPlayer.NextItem();
		else if (event.button == nwf.input.ControllerButton.PRO_UP && !WUMPlayer.ItemListMouseOver)
			WUMPlayer.PreviousItem();
		else if (event.button == nwf.input.ControllerButton.PRO_A && WUMPlayer.ItemIndex != -1)
			WUMPlayer.SelectItem();
		else if (event.button == nwf.input.ControllerButton.PRO_B)
			WUMPlayer.Back();
		else if (event.button == nwf.input.ControllerButton.PRO_X && WUMPlayer.Context == 'playing audio')
			WUMPlayer.ChangeRandomAudio();
		else if (event.button == nwf.input.ControllerButton.PRO_Y && WUMPlayer.Context == 'playing audio')
			WUMPlayer.PlayPauseAudio();
		else if (event.button == nwf.input.ControllerButton.PRO_R && WUMPlayer.Context == 'playing audio')
			WUMPlayer.NextAudio();
	}
	else if (WUMPlayer.Context == 'playing video') {
		if (event.button == nwf.input.ControllerButton.PRO_B)
			WUMPlayer.Back();
	}
	else if (WUMPlayer.Context == 'showing image') {
		if (event.button == nwf.input.ControllerButton.PRO_L)
			WUMPlayer.PreviousImage();
		else if (event.button == nwf.input.ControllerButton.PRO_R)
			WUMPlayer.NextImage();
		else if (event.button == nwf.input.ControllerButton.PRO_B)
			WUMPlayer.Back();
		else if (event.button == nwf.input.ControllerButton.PRO_Y)
			WUMPlayer.ResetImageScene();
		else if (event.button == nwf.input.ControllerButton.PRO_X)
			WUMPlayer.ChangeImageMode();
	}
};

Pro.JoystickLeftMove = function(event) {
    if (Math.abs(event.movementX) < 0.05) {
        event.movementX = 0;
    }
	if (Math.abs(event.movementY) < 0.05) {
        event.movementY = 0;
    }
	
	if (WUMPlayer.Context == 'showing image') {
		var bg = document.getElementById("image_bg");
		
		if (event.movementX < -0.80 && bg.scrollLeft > 0)
			WUMPlayer.MoveImageLeft();
		else if (event.movementX > 0.80 && bg.scrollLeft < bg.scrollWidth)
			WUMPlayer.MoveImageRight();
		
		if (event.movementY < -0.80 && bg.scrollTop > 0)
			WUMPlayer.MoveImageUp();
		else if (event.movementY > 0.80 && bg.scrollTop < bg.scrollHeight)
			WUMPlayer.MoveImageDown();
	}
};

Pro.JoystickRightMove = function(event) {
    if (Math.abs(event.movementX) < 0.05) {
        event.movementX = 0;
    }
	if (Math.abs(event.movementY) < 0.05) {
        event.movementY = 0;
    }
	
	if (WUMPlayer.Context == 'showing image' && WUMPlayer.ImageMode == 'scale') {
		if (event.movementY < -0.95)
			WUMPlayer.IncreaseImageScale();
		else if (event.movementY > 0.95)
			WUMPlayer.DecreaseImageScale();
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