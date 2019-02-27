/*
Copyright (c) 2003-2012, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function( config )
{
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';

    var roxyFileman = '/fileman/index.html?integration=ckeditor';
    config.filebrowserBrowseUrl = roxyFileman;
    config.filebrowserImageBrowseUrl = roxyFileman + '&type=image';
    config.removeDialogTabs = 'link:upload;image:upload';

    //config.syntaxhighlight_lang = 'csharp';
    //config.syntaxhighlight_hideControls = true;
    //config.languages = 'vi';
    //config.filebrowserBrowseUrl = '/Scripts/ckfinder/ckfinder.html';
    //config.filebrowserImageBrowseUrl = '/Scripts/ckfinder/ckfinder.html?Types=Images';
    //config.filebrowserFlashBrowseUrl = '/Scripts/ckfinder/ckfinder.html?Types=Flash';
    //config.filebrowserUploadUrl = '/Scripts/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=File';
    //config.filebrowserImageUploadUrl = '/Scripts/Data';
    //config.filebrowserFlashUploadUrl = '/Scripts/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Flash';

    //config.removeDialogTabs = 'image:advanced;link:advanced';
    //config.image_previewText = ' ';

    //CKFinder.setupCKEditor(null, '/Scripts/ckfinder/');
};
