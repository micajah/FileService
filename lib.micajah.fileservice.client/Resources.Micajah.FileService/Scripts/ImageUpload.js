Type.registerNamespace("Micajah.FileService");

Micajah.FileService.ImageUpload = function(a) {
    Micajah.FileService.ImageUpload.initializeBase(this, [a]);
    this._allowedFileExtensions = "";
    this._closePopupWindowButtonId = this.get_id() + "_ClosePopupWindowButton";
    this._deletingConfirmationText = "";
    this._elementsToSynchronize = [["OriginalFileField", "value", false], ["CurrentFileField", "value", true], ["PreviousFilesField", "value", true], ["UploadedImage", "src", true]];
    this._enablePopupWindow = true;
    this._errorDivId = this.get_id() + "_ErrorDiv";
    this._errorMessages = [];
    this._panelCount = 0;
    this._popupWindow = null;
    this._uploadButtonId = this.get_id() + "_UploadButton";
    this._uploadType = -1;
    this._handleCloseButtonClickDelegate = null;
    this._handleDeleteButtonClickDelegate = Function.createDelegate(this, this._handleDeleteButtonClick);
    this._handleUploadButtonClickDelegate = Function.createDelegate(this, this._handleUploadButtonClick);
    this._handleUploadedImageClickDelegate = Function.createDelegate(this, this._handleUploadedImageClick);
    this._handleUploadTypeListChangedDelegate = Function.createDelegate(this, this._handleUploadTypeListChanged);
    this.isPostBack = false;
}

Micajah.FileService.ImageUpload.prototype =
{
    get_allowedFileExtensions: function() { return this._allowedFileExtensions; },
    set_allowedFileExtensions: function(a) { this._allowedFileExtensions = a; },

    get_deletingConfirmationText: function() { return this._deletingConfirmationText; },
    set_deletingConfirmationText: function(a) { this._deletingConfirmationText = a; },

    get_enablePopupWindow: function() { return this._enablePopupWindow; },
    set_enablePopupWindow: function(a) { this._enablePopupWindow = a; },

    get_errorMessages: function() { return this._errorMessages; },
    set_errorMessages: function(a) {
        if (a)
            this._errorMessages = eval(a);
        else
            this._errorMessages = [];
    },

    _addHandler: function(elementId, eventName, handler) {
        var element = $get(elementId);
        if (element && handler) $addHandler(element, eventName, handler);
    },

    _getElementIdWithPrefix: function(buttonId, prefix) {
        if (typeof (prefix) != "string") prefix = this.get_id();
        return prefix + "_" + buttonId;
    },

    _getDeleteButtonId: function(prefix) {
        return this._getElementIdWithPrefix("DeleteButton", prefix);
    },

    _getFileExtension: function(filePath) { return this._getLastPartOfString(filePath, ".").toLowerCase(); },

    _getLastPartOfString: function(str, delimiter) {
        if (str.indexOf(delimiter) > -1) {
            var array1 = str.split(delimiter);
            return array1[array1.length - 1];
        }
        return str;
    },

    _getPopupWindowParent: function() {
        return this._popupWindow.get_element().parentNode;
    },

    _getPopupWindowParentPrefix: function() {
        return this._popupWindow.get_id().replace("_PopupWindow", "");
    },

    _getRadWindow: function() {
        var wnd = null;
        if (window.radWindow) wnd = window.radWindow;
        else if (window.frameElement && window.frameElement.radWindow) wnd = window.frameElement.radWindow;
        if (wnd) {
            var wndId = wnd.get_id();
            var str = "_PopupWindow";
            if (wndId.substr(wndId.length - str.length) != str) wnd = null;
        }
        return wnd;
    },

    _getUploadedImageId: function(prefix) {
        return this._getElementIdWithPrefix("UploadedImage", prefix);
    },

    _disposeUploadTypeList: function() {
        var inputs = this.get_element().getElementsByTagName("INPUT");
        for (var i = 0; i < inputs.length; i++) {
            if (inputs[i].type == "radio") $removeHandler(inputs[i], "click", this._handleUploadTypeListChangedDelegate);
        }
        this._handleUploadTypeListChangedDelegate = null;
    },

    _initializeButtons: function() {
        if (this._popupWindow) {
            this._handleCloseButtonClickDelegate = Function.createDelegate(this, this._handleCloseButtonClick);
            this._addHandler(this._closePopupWindowButtonId, "click", this._handleCloseButtonClickDelegate);
            $get(this._closePopupWindowButtonId).style.display = "inline";
            var separator = $get(this.get_id() + "_ButtonSeparator");
            separator.style.display = "inline";
            separator.parentNode.insertBefore($get(this._uploadButtonId), separator);
        }
    },

    _initializeUploadTypeList: function() {
        this._panelCount = 0;
        var inputs = this.get_element().getElementsByTagName("INPUT");
        var index = -1;
        for (var i = 0; i < inputs.length; i++) {
            if (inputs[i].type == "radio") {
                this._panelCount++;
                if (inputs[i].checked) index = parseInt(inputs[i].value);
                $addHandler(inputs[i], "click", this._handleUploadTypeListChangedDelegate);
            }
        }
        this._showPanel(index);
    },

    _handleCloseButtonClick: function(args) {
        if (this._popupWindow) this._popupWindow.close();
    },

    _handleDeleteButtonClick: function(args) {
        if (this._deletingConfirmationText.length > 0) {
            if (window.confirm(this._deletingConfirmationText) == false)
                return;
        }

        $get(this.get_id() + "_CurrentFileField").value = "";
        if (!this.isPostBack) {
            this.isPostBack = true;
            this._synchronize();
            this.isPostBack = false;
        }
        else
            this._synchronize();
        this._loadUploadedImage();
    },

    _handleUploadButtonClick: function(args) {
        var errorDiv = $get(this._errorDivId);
        errorDiv.style.display = "none";
        var filePathIsValid = false;
        var fileFromMyComputer = $get(this.get_id() + "_FileFromMyComputer");
        var fileFromWeb = $get(this.get_id() + "_FileFromWeb");
        var errorIndex = -1;
        var filePath = null;
        switch (this._uploadType) {
            case 0:
                filePath = fileFromMyComputer.value;
                filePathIsValid = (!this._stringIsEmpty(filePath));
                if (filePathIsValid) {
                    filePathIsValid = this._validateExtension(filePath);
                    if (filePathIsValid)
                        fileFromWeb.value = "";
                    else
                        errorIndex = 3;
                }
                else
                    errorIndex = 0;
                break;
            case 1:
                filePath = fileFromWeb.value;
                filePathIsValid = (!this._stringIsEmpty(filePath));
                if (filePathIsValid) {
                    filePathIsValid = this._validateUrl(filePath);
                    if (filePathIsValid) {
                        filePathIsValid = this._validateExtension(filePath);
                        if (filePathIsValid)
                            fileFromMyComputer.parentNode.removeChild(fileFromMyComputer);
                        else
                            errorIndex = 3;
                    }
                    else
                        errorIndex = 2;
                }
                else
                    errorIndex = 1;
                break;
        }
        if (!filePathIsValid) {
            errorDiv.innerHTML = ((errorIndex < this._errorMessages.length) ? this._errorMessages[errorIndex] : "");
            errorDiv.style.display = "block";
            args.preventDefault();
            args.stopPropagation();
        }
    },

    _handleUploadedImageClick: function(args) {
        if (!this._enablePopupWindow)
            window.open($get(this._getUploadedImageId()).src.split("&Width=")[0], "_blank");
    },

    _handleUploadTypeListChanged: function(args) {
        this._showPanel(parseInt(args.target.value));
    },

    _loadUploadedImage: function() {
        var parent = null;
        var prefix = null;
        if (arguments.length == 0)
            prefix = this.get_id();
        else {
            parent = arguments[0];
            prefix = arguments[1];
        }
        var uploadedImage = $get(this._getUploadedImageId(prefix), parent);
        var noImageLabel = $get(prefix + "_NoImageLabel", parent);
        var currentFileField = $get(prefix + "_CurrentFileField", parent);
        var deleteButton = $get(this._getDeleteButtonId(prefix), parent);
        if (currentFileField.value.length > 0) {
            uploadedImage.style.display = "inline";
            if (deleteButton) deleteButton.style.display = "inline";
            noImageLabel.style.display = "none";
        }
        else {
            uploadedImage.style.display = "none";
            if (deleteButton) deleteButton.style.display = "none";
            noImageLabel.style.display = "inline";
        }
        if ((arguments.length == 0) && (this._popupWindow))
            this._loadUploadedImage(this._getPopupWindowParent(), this._getPopupWindowParentPrefix());
    },

    _removeHandler: function(elementId, eventName, handler) {
        var element = $get(elementId);
        if (element && handler) $removeHandler(element, eventName, handler);
    },

    _stringIsEmpty: function(str) {
        return (str.replace(/^\s+$/gm, "").length == 0);
    },

    _synchronize: function() {
        if (this._popupWindow) {
            var parentElementIdPrefix = this._getPopupWindowParentPrefix();
            var parent = this._getPopupWindowParent();
            var elementId = null;
            for (var i in this._elementsToSynchronize) {
                elementId = this._elementsToSynchronize[i][0];
                var element = $get(this.get_id() + "_" + elementId);
                var parentElement = $get(parentElementIdPrefix + "_" + elementId, parent);
                var enableSyncAfterPostBack = this._elementsToSynchronize[i][2];
                if (element && parentElement) {
                    var value = null;
                    if (this.isPostBack && enableSyncAfterPostBack) {
                        value = element.getAttribute(this._elementsToSynchronize[i][1]);
                        if (typeof (value) != "string") value = "";
                        parentElement.setAttribute(this._elementsToSynchronize[i][1], value);
                    }
                    else {
                        value = parentElement.getAttribute(this._elementsToSynchronize[i][1]);
                        if (typeof (value) != "string") value = "";
                        element.setAttribute(this._elementsToSynchronize[i][1], value);
                    }
                }
            }
        }
    },

    _showPanel: function(index) {
        if ((!isNaN(index)) && (index > -1)) {
            this._uploadType = index;
            for (var i = 0; i < this._panelCount; i++) {
                $get(this.get_id() + "_Panel" + i).style.display = ((i == index) ? "block" : "none");
            }
        }
    },

    _validateExtension: function(filePath) {
        return (("," + this._allowedFileExtensions + ",").indexOf("," + this._getFileExtension(filePath) + ",") > -1);
    },

    _validateUrl: function(filePath) {
        var re = new RegExp("^http(s)?://([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?$", "i");
        return re.test(filePath);
    },

    dispose: function() {
        this._removeHandler(this._closePopupWindowButtonId, "click", this._handleCloseButtonClickDelegate);
        this._removeHandler(this._uploadButtonId, "click", this._handleUploadButtonClickDelegate);
        this._removeHandler(this._getUploadedImageId(), "click", this._handleUploadedImageClickDelegate);
        this._removeHandler(this._getDeleteButtonId(), "click", this._handleDeleteButtonClickDelegate);
        this._handleCloseButtonClickDelegate = null;
        this._handleUploadButtonClickDelegate = null;
        this._handleUploadedImageClickDelegate = null;
        this._handleDeleteButtonClickDelegate = null;
        this._disposeUploadTypeList();
        Micajah.FileService.ImageUpload.callBaseMethod(this, "dispose");
    },

    initialize: function() {
        Micajah.FileService.ImageUpload.callBaseMethod(this, "initialize");
        this._popupWindow = this._getRadWindow();
        this._initializeUploadTypeList();
        this._initializeButtons();
        this._synchronize();
        this._addHandler(this._getDeleteButtonId(), "click", this._handleDeleteButtonClickDelegate);
        this._addHandler(this._uploadButtonId, "click", this._handleUploadButtonClickDelegate);
        this._addHandler(this._getUploadedImageId(), "click", this._handleUploadedImageClickDelegate);
        this._loadUploadedImage();
    }
}

Micajah.FileService.ImageUpload.registerClass("Micajah.FileService.ImageUpload", Sys.UI.Control);