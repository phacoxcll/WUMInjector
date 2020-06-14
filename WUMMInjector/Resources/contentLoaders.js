var content = JSON.parse(contentJSON);

function loadRoot() {
	loadFolder(content.foldername, 'all');
}

function loadFolder(path, contentType) {
	document.getElementById('content').style.marginLeft = '192px';
	document.getElementById('menu').style.display = 'inherit';
	
	if (contentType == 'all') {
		var menu = '<li id="menu_all" class="menu_active"><a href="#" onclick="loadFolder(\'' + path + '\', \'all\')"><img width="192" height="192" src="all.png" /></a></li>';
		menu += '<li id="menu_video"><a href="#" onclick="loadFolder(\'' + path + '\', \'video\')"><img width="192" height="192" src="video.png" /></a></li>';
		menu += '<li id="menu_audio"><a href="#" onclick="loadFolder(\'' + path + '\', \'audio\')"><img width="192" height="192" src="audio.png" /></a></li>';
		menu += '<li id="menu_image"><a href="#" onclick="loadFolder(\'' + path + '\', \'image\')"><img width="192" height="192" src="image.png" /></a></li>';
		document.getElementById('menu').innerHTML = menu;
	}
	else if (contentType == 'video') {
		var menu = '<li id="menu_all"><a href="#" onclick="loadFolder(\'' + path + '\', \'all\')"><img width="192" height="192" src="all.png" /></a></li>';
		menu += '<li id="menu_video" class="menu_active"><a href="#" onclick="loadFolder(\'' + path + '\', \'video\')"><img width="192" height="192" src="video.png" /></a></li>';
		menu += '<li id="menu_audio"><a href="#" onclick="loadFolder(\'' + path + '\', \'audio\')"><img width="192" height="192" src="audio.png" /></a></li>';
		menu += '<li id="menu_image"><a href="#" onclick="loadFolder(\'' + path + '\', \'image\')"><img width="192" height="192" src="image.png" /></a></li>';
		document.getElementById('menu').innerHTML = menu;
	}
	else if (contentType == 'audio') {
		var menu = '<li id="menu_all"><a href="#" onclick="loadFolder(\'' + path + '\', \'all\')"><img width="192" height="192" src="all.png" /></a></li>';
		menu += '<li id="menu_video"><a href="#" onclick="loadFolder(\'' + path + '\', \'video\')"><img width="192" height="192" src="video.png" /></a></li>';
		menu += '<li id="menu_audio" class="menu_active"><a href="#" onclick="loadFolder(\'' + path + '\', \'audio\')"><img width="192" height="192" src="audio.png" /></a></li>';
		menu += '<li id="menu_image"><a href="#" onclick="loadFolder(\'' + path + '\', \'image\')"><img width="192" height="192" src="image.png" /></a></li>';
		document.getElementById('menu').innerHTML = menu;
	}
	else if (contentType == 'image') {
		var menu = '<li id="menu_all"><a href="#" onclick="loadFolder(\'' + path + '\', \'all\')"><img width="192" height="192" src="all.png" /></a></li>';
		menu += '<li id="menu_video"><a href="#" onclick="loadFolder(\'' + path + '\', \'video\')"><img width="192" height="192" src="video.png" /></a></li>';
		menu += '<li id="menu_audio"><a href="#" onclick="loadFolder(\'' + path + '\', \'audio\')"><img width="192" height="192" src="audio.png" /></a></li>';
		menu += '<li id="menu_image" class="menu_active"><a href="#" onclick="loadFolder(\'' + path + '\', \'image\')"><img width="192" height="192" src="image.png" /></a></li>';
		document.getElementById('menu').innerHTML = menu;
	}
	
	var folder = getFolder(path);
	
	var folders = path.split("/");
	if (folders.length > 1 && folders[folders.length - 1] == '..') {
		if (folders.length > 2)
			folders.pop();
		folders.pop();
		path = folders.join("/");
	}
	
	var content = '<h3 class="list">' + path + '</h3><ul class="list">';
	content += '<li class="list"><a href="#" onclick="loadFolder(\'' + path + '/' + '..\', \'' + contentType + '\')">..</a></li>';
	var i;
	for (i = 0; i < folder.folders.length; i++) {
		content += '<li class="list"><a href="#" onclick="loadFolder(\'' + path + '/' + folder.folders[i].foldername + '\', \'' + contentType + '\')">' + folder.folders[i].name + '</a></li>';
	}
	for (i = 0; i < folder.files.length; i++) {
		if ((contentType == 'all' || contentType == 'video') && folder.files[i].ext == '.mp4') {
			content += '<li id="index' + i + '" class="list"><a href="#" onclick="loadVideo(\'' + path + '\', ' + i + ', \'' + contentType + '\')">' + folder.files[i].name + '</a></li>';
		}
		else if ((contentType == 'all' || contentType == 'audio') && (folder.files[i].ext == '.m4a' ||folder.files[i].ext == '.ogg')) {
			content += '<li id="index' + i + '" class="list"><a href="#" onclick="loadAudio(\'' + path + '\', ' + i + ', \'' + contentType + '\')">' + folder.files[i].name + '</a></li>';
		}
		else if ((contentType == 'all' || contentType == 'image') && (folder.files[i].ext == '.jpg' || folder.files[i].ext == '.png' || folder.files[i].ext == '.gif' || folder.files[i].ext == '.bmp')) {
			content += '<li id="index' + i + '" class="list"><a href="#" onclick="loadImage(\'' + path + '\', ' + i + ', \'' + contentType + '\')">' + folder.files[i].name + '</a></li>';
		}
	}
	content += '</ul>';
	
	document.getElementById('content').innerHTML = content;
}

function getFolder(path) {
	var folder = content;
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
	
	return folder;
}

function loadVideo(path, index, contentType) {
	var folder = getFolder(path);
	var content = '<video id="video" onended="endVideoFunctionT(\'' + path + '\', ' + index + ', \'' + contentType + '\')" controls autoplay><source src="' + path + '/' + folder.files[index].filename + folder.files[index].ext + '" type="video/mp4">Your browser does not support the video tag.</video><h1 id="title">' + folder.files[index].name + '</h1><a id="backVideo" href="#" onclick="loadFolder(\'' + path + '\', \'' + contentType + '\')">Regresar</a>';
	document.getElementById('menu').style.display = 'none';
	document.getElementById('content').style.marginLeft = '0';
	document.getElementById('content').innerHTML = content;
}

function endVideoFunctionT(path, index, contentType) {
	setTimeout(endVideoFunction, 1000, path, index, contentType);
}

function endVideoFunction(path, index, contentType) {
	var folder = getFolder(path);
	var i = index + 1;
	if (i == folder.files.length)
		i = 0;
	while (folder.files[i].ext != '.mp4') {
		i++;
		if (i == folder.files.length)
			i = 0;
	}
	loadVideo(path, i, contentType);
}

function loadAudio(path, index, contentType) {
	var folder = getFolder(path);
	var content = '<h3 class="list">' + folder.files[index].name + '</h3><ul class="list">';
	content += '<li class="list"><a href="#" onclick="loadFolder(\'' + path + '/' + '..\', \'' + contentType + '\')">..</a></li>';
	var i;
	for (i = 0; i < folder.folders.length; i++) {
		content += '<li class="list"><a href="#" onclick="loadFolder(\'' + path + '/' + folder.folders[i].foldername + '\', \'' + contentType + '\')">' + folder.folders[i].name + '</a></li>';
	}
	if (contentType == 'all') {
		for (i = 0; i < folder.files.length; i++) {
			if (folder.files[i].ext == '.mp4') {
				content += '<li id="index' + i + '" class="list"><a href="#" onclick="loadVideo(\'' + path + '\', ' + i + ', \'' + contentType + '\')">' + folder.files[i].name + '</a></li>';
			}
			else if (folder.files[i].ext == '.m4a' || folder.files[i].ext == '.ogg') {
				content += '<li id="index' + i + '" class="list"><a href="#" onclick="loadAudio(\'' + path + '\', ' + i + ', \'' + contentType + '\')">' + folder.files[i].name + '</a></li>';
			}
			else if (folder.files[i].ext == '.jpg' || folder.files[i].ext == '.png' || folder.files[i].ext == '.gif' || folder.files[i].ext == '.bmp') {
				content += '<li id="index' + i + '" class="list"><a href="#" onclick="loadImage(\'' + path + '\', ' + i + ', \'' + contentType + '\')">' + folder.files[i].name + '</a></li>';
			}
		}
	}
	else if (contentType == 'audio') {
		for (i = 0; i < folder.files.length; i++) {
			if (folder.files[i].ext == '.m4a' || folder.files[i].ext == '.ogg') {
				content += '<li id="index' + i + '" class="list"><a href="#" onclick="loadAudio(\'' + path + '\', ' + i + ', \'' + contentType + '\')">' + folder.files[i].name + '</a></li>';
			}
		}
	}
	
	content += '</ul>';
	document.getElementById('content').innerHTML = content;
	
	var audio = '';
	if (folder.files[index].ext == '.m4a')
		audio = '<audio id="audio" onended="endAudioFunction(\'' + path + '\', ' + index + ', \'' + contentType + '\')" controls autoplay><source src="' + path + '/' + folder.files[index].filename + folder.files[index].ext + '" type="audio/aac">Your browser does not support the audio tag.</audio>';
	else if (folder.files[index].ext == '.ogg')
		audio = '<audio id="audio" onended="endAudioFunction(\'' + path + '\', ' + index + ', \'' + contentType + '\')" controls autoplay><source src="' + path + '/' + folder.files[index].filename + folder.files[index].ext + '" type="audio/ogg">Your browser does not support the audio tag.</audio>';
	document.getElementById('index' + index).innerHTML = audio;
}

function endAudioFunction(path, index, contentType) {
	var folder = getFolder(path);
	var i = index + 1;
	if (i == folder.files.length)
		i = 0;
	while (folder.files[i].ext != '.m4a' && folder.files[i].ext != '.ogg') {
		i++;
		if (i == folder.files.length)
			i = 0;
	}
	loadAudio(path, i, contentType);
}

function loadImage(path, index, contentType) {
	var folder = getFolder(path);
	var content = '<div id="image_bg" onclick="endImageFunction(\'' + path + '\', ' + index + ', \'' + contentType + '\')"><img id="image" src="' + path + '/' + folder.files[index].filename + folder.files[index].ext + '" /></div><h1 id="title">' + folder.files[index].name + '</h1><a id="backImage" href="#" onclick="loadFolder(\'' + path + '\', \'' + contentType + '\')">Regresar</a>';
	document.getElementById('menu').style.display = 'none';
	document.getElementById('content').style.marginLeft = '0';
	document.getElementById('content').innerHTML = content;
	
	var windowWidth = window.innerWidth
	|| document.documentElement.clientWidth
	|| document.body.clientWidth;
	var windowHeight = window.innerHeight
	|| document.documentElement.clientHeight
	|| document.body.clientHeight;
	
	var widthScale = windowWidth / folder.files[index].width;
	var heightScale = windowHeight / folder.files[index].height;
	
	var width = folder.files[index].width;
	var height = folder.files[index].height;
	
	if (widthScale < 1 || heightScale < 1) {
		if (widthScale < heightScale) {
			width = Math.floor(folder.files[index].width * widthScale);
			height = Math.floor(folder.files[index].height * widthScale);
		}
		else {
			width = Math.floor(folder.files[index].width * heightScale);
			height = Math.floor(folder.files[index].height * heightScale);
		}
	}
	
	var positionX = Math.floor((windowWidth - width) / 2);
	var positionY = Math.floor((windowHeight - height) / 2);
	
	document.getElementById("image_bg").style.width = windowWidth + 'px';
	document.getElementById("image_bg").style.height = windowHeight + 'px';
	
	document.getElementById("image").style.width = width + 'px';
	document.getElementById("image").style.height = height + 'px';
	
	document.getElementById("image").style.right = positionX + 'px';
	document.getElementById("image").style.top = positionY + 'px';
}

function endImageFunction(path, index, contentType) {
	var folder = getFolder(path);
	var i = index + 1;
	if (i == folder.files.length)
		i = 0;
	while (folder.files[i].ext != '.jpg' && folder.files[i].ext != '.png' && folder.files[i].ext != '.gif' && folder.files[i].ext != '.bmp') {
		i++;
		if (i == folder.files.length)
			i = 0;
	}
	loadImage(path, i, contentType);
}