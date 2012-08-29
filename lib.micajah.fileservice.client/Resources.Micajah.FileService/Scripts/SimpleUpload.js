Type.registerNamespace("Micajah.FileService");

Micajah.FileService.SimpleUpload = function (a) {
    Micajah.FileService.SimpleUpload.initializeBase(this, [a]);
    this._elementsToSynchronize = [["DeletedFilesField", "value", true], ["UploadedFilesField", "value", true], ["UploadedByFlashFilesField", "value", true], ["FilesMetaDataField", "value", true], ["CurrentModeField", "value", true]];
    this._enableFileService = true;
    this._messages = [];
    this._flashIsEnabled = null;
    this._fileTypeIcons = [];
    this._fileTypeIconUrlFormat = "";
    this._filesCount = 0;
    this._inputTabIndex = 0;
    this._inputWidth = null;
    this._popupWindow = null;
    this._showUploadedFiles = true;
    this._showProgressArea = false;
    this._swfUpload = null;
    this._swfUploadFlashUrl = "";
    this._swfUploadButtonText = "";
    this._swfUploadFileSizeLimit = 0;
    this._swfUploadFileUploadUrl = "";
    this._uploadButtonAcceptChanges = false;
    this._uploadControlsUniqueId = [];
    this._handleCloseButtonClickDelegate = null;
    this.isPostBack = false;
}

Micajah.FileService.SimpleUpload.prototype =
{
    _get_changeModeMessageHolder: function () { return $get(this.get_id() + "_ChangeModeMessageHolder"); },

    _get_currentModeField: function () { return $get(this.get_id() + "_CurrentModeField"); },

    _get_deletedFilesField: function () { return $get(this.get_id() + "_DeletedFilesField"); },

    _get_eventTarget: function () {
        var target = null;
        var args = window.event;
        if (args) {
            target = (args.target ? args.target : args.srcElement);
            if (target.nodeType == 3) target = target.parentNode;
        }
        return target;
    },

    _get_lastListItem: function () {
        var items = this._getListElement().getElementsByTagName("li");
        return items[items.length - 1];
    },

    _get_popupWindow: function () {
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

    _get_swfUploadHolder: function () { return $get(this.get_id() + "_FlashUploadHolder"); },

    _get_validator: function () { return $get(this.get_id() + "_Validator"); },

    _get_uploadedFilesField: function () { return $get(this.get_id() + "_UploadedFilesField"); },

    _get_uploadHolder: function () { return $get(this.get_id() + "_UploadHolder"); },

    get_availableFilesCount: function () {
        var availableFilesCount = (this.get_maxFileCount() - this.get_filesCount());
        if (availableFilesCount < 0) availableFilesCount = 0;
        return availableFilesCount;
    },

    get_currentMode: function () {
        var currentModeField = this._get_currentModeField();
        var mode = parseInt(currentModeField.value);
        if (isNaN(mode)) mode = 0;
        return mode;
    },

    get_enableFileService: function () { return this._enableFileService; },
    set_enableFileService: function (a) { this._enableFileService = a; },

    get_flashIsEnabled: function () {
        if (this._flashIsEnabled == null) {
            if (typeof (DetectFlashVer) == "function")
                this._flashIsEnabled = DetectFlashVer(9, 0, 0);
            else
                this._flashIsEnabled = false;
        }
        return this._flashIsEnabled;
    },

    get_messages: function () { return this._messages; },
    set_messages: function (a) {
        if (a)
            this._messages = eval(a);
        else
            this._messages = [];
    },

    get_fileTypeIcons: function () { return this._fileTypeIcons; },
    set_fileTypeIcons: function (a) {
        if (a)
            this._fileTypeIcons = eval(a);
        else
            this._fileTypeIcons = [];
    },

    get_fileTypeIconUrlFormat: function () { return this._fileTypeIconUrlFormat; },
    set_fileTypeIconUrlFormat: function (a) { this._fileTypeIconUrlFormat = a; },

    get_filesCount: function () { return this._filesCount; },

    get_inputTabIndex: function () { return this._inputTabIndex; },
    set_inputTabIndex: function (a) { this._inputTabIndex = a; },

    get_inputWidth: function () { return this._inputWidth; },
    set_inputWidth: function (a) { this._inputWidth = a; },

    get_notUploadedFilesCount: function () {
        var count = 0;
        var listItems = this._getListElement().getElementsByTagName("li");
        for (var i = 0; i < listItems.length; i++) {
            if (eval(listItems[i].getAttribute("fileIsNotUploadedYet")) == true) count++;
        }
        return count;
    },

    get_swfUploadFlashUrl: function () { return this._swfUploadFlashUrl; },
    set_swfUploadFlashUrl: function (a) { this._swfUploadFlashUrl = a; },

    get_swfUploadButtonText: function () { return this._swfUploadButtonText; },
    set_swfUploadButtonText: function (a) { this._swfUploadButtonText = a; },

    get_swfUploadFileSizeLimit: function () { return this._swfUploadFileSizeLimit; },
    set_swfUploadFileSizeLimit: function (a) { this._swfUploadFileSizeLimit = a; },

    get_swfUploadFileUploadUrl: function () { return this._swfUploadFileUploadUrl; },
    set_swfUploadFileUploadUrl: function (a) { this._swfUploadFileUploadUrl = a; },

    get_showUploadedFiles: function () { return this._showUploadedFiles; },
    set_showUploadedFiles: function (a) { this._showUploadedFiles = a; },

    get_showProgressArea: function () { return this._showProgressArea; },
    set_showProgressArea: function (a) { this._showProgressArea = a; },

    get_uploadButtonAcceptChanges: function () { return this._uploadButtonAcceptChanges; },
    set_uploadButtonAcceptChanges: function (a) { this._uploadButtonAcceptChanges = a; },

    get_uploadControlsUniqueId: function () { return this._uploadControlsUniqueId; },
    set_uploadControlsUniqueId: function (a) {
        if (a)
            this._uploadControlsUniqueId = eval(a);
        else
            this._uploadControlsUniqueId = [];
    },

    _addHandler: function (elementId, eventName, handler) {
        var element = $get(elementId);
        if (element && handler) $addHandler(element, eventName, handler);
    },

    _addUploadedByFlashFile: function (metaData) {
        var field = $get(this.get_id() + "_UploadedByFlashFilesField");
        if (field.value.length > 0)
            field.value += "|" + metaData;
        else
            field.value = metaData;
    },

    _appendFile: function (filePath, fileUniqueId, fileUrl, index) {
        var listItem = this._addRow(null);
        $addHandlers(listItem, { "click": this._rowClicked }, this);
        if (fileUniqueId) listItem.setAttribute("fileUniqueId", fileUniqueId);
        this._appendFileTypeIcon(listItem, filePath);

        var createFileLink = (fileUniqueId && fileUrl && this._enableFileService);
        var fileNameElement = document.createElement(createFileLink ? "A" : "SPAN");
        if (fileUrl)
            fileNameElement.className = "suFileWrap";
        else {
            fileNameElement.title = this._messages[1];
            fileNameElement.className = "suFileWrapAdded";
            listItem.setAttribute("fileIsNotUploadedYet", true);
            if (!isNaN(parseInt(index))) listItem.setAttribute("fileIndex", index);
        }

        var text = this._getFileName(filePath);
        var tootTip = "";
        if (text.length > 52) {
            tootTip = text;
            text = text.substr(0, 45) + "...." + this._getFileExtension(text);
        }
        fileNameElement.innerHTML = text;
        if (tootTip.length > 0) fileNameElement.setAttribute("title", tootTip);
        if (createFileLink) {
            fileNameElement.target = "_blank";
            fileNameElement.href = fileUrl;
        }
        listItem.appendChild(fileNameElement);

        var removeButton = this.appendRemoveButton(listItem);
        if (navigator.userAgent.indexOf("MSIE") > -1) {
            removeButton.onmouseover = function () { this.className += " hover"; };
            removeButton.onmouseout = function () { this.className = this.className.replace(" hover", ""); };
        }
        else
            removeButton.setAttribute("onclick", "window.event = event;");
    },

    _appendFileTypeIcon: function (listItem, filePath) {
        var fileTypeIcon = document.createElement("IMG");
        fileTypeIcon.className = "suFileTypeIcon";
        fileTypeIcon.src = this._getFileTypeIconUrl(filePath);
        listItem.appendChild(fileTypeIcon);
    },

    _cancelEvent: function (e) {
        if ((e.type == "keydown") && (e.keyCode == 9))
            return true;
        else
            return Micajah.FileService.SimpleUpload.callBaseMethod(this, "_cancelEvent", [e]);
    },

    _changeModeMessage: function () {
        var holder = this._get_changeModeMessageHolder();
        if (holder) {
            if (this.get_flashIsEnabled() == true) {
                var mode = this.get_currentMode();
                holder.innerHTML = this._messages[((mode == 0) ? 2 : 3)].replace("<a", "<a onclick=\"$find('" + this.get_id() + "').changeMode(" + ((mode == 0) ? 1 : 0) + ");return false;\"");
            }
            else
                holder.style.display = "none";
        }
    },

    _deleteUploadedFile: function (index, fileUniqueId, fileIsNotUploadedYet) {
        if (fileIsNotUploadedYet == true) {
            var fileCancelled = false;
            if (this._swfUpload) {
                if (this._swfUpload.getFile(fileUniqueId)) {
                    fileCancelled = true;
                    this._swfUpload.cancelUpload(fileUniqueId);
                }
            }
            if (fileCancelled == false) {
                var fields = this.getFileInputs();
                for (var i = 0; i < fields.length; i++) {
                    var inp = fields[i];
                    if (inp.id == (this.get_id() + "file" + index)) {
                        $clearHandlers(inp);
                        inp.parentNode.removeChild(inp);
                        break;
                    }
                }
            }
        }
        else if (this._enableFileService) {
            var uploadedFilesField = this._get_uploadedFilesField();
            var array1 = uploadedFilesField.value.split("|");
            if (array1.length > index) {
                var deletedFileUniqueId = array1.splice(index, 1)[0].split("*")[0];
                uploadedFilesField.value = array1.join("|");
                var deletedFilesField = this._get_deletedFilesField();
                if (deletedFilesField.value.length > 0)
                    deletedFilesField.value += "|" + deletedFileUniqueId;
                else
                    deletedFilesField.value = deletedFileUniqueId;
            }
        }
        if (!this.get_uploadButtonAcceptChanges()) this._synchronize(true);
        this._setFilesCount(-1);
    },

    _getFileName: function (filePath) { return this._getLastPartOfString(filePath, "\\"); },

    _getFileExtension: function (filePath) { return this._getLastPartOfString(filePath, ".").toLowerCase(); },

    _getFileTypeIconUrl: function (filePath) {
        var fileExtension = this._getFileExtension(filePath);
        var fileTypeIconUrl = "";
        for (var i in this._fileTypeIcons) {
            if (i == 0) fileTypeIconUrl = this._fileTypeIcons[i][1];
            if (this._fileTypeIcons[i][0].toLowerCase() == fileExtension) {
                fileTypeIconUrl = this._fileTypeIcons[i][1];
                break;
            }
        }
        return this._fileTypeIconUrlFormat.replace("{0}", fileTypeIconUrl);
    },

    _getLastPartOfString: function (str, delimiter) {
        if (str.indexOf(delimiter) > -1) {
            var array1 = str.split(delimiter);
            return array1[array1.length - 1];
        }
        return str;
    },

    _getSwfUploadButtonWidth: function () {
        var buttonWidth = null;
        var fields = this.getFileInputs();
        var fieldsCount = fields.length;
        var fromStyle = false;
        if (fieldsCount > 0) {
            var field = fields[fieldsCount - 1];
            if ((field.style != null) && (field.style.width != null) && (field.style.width.indexOf("px") > -1))
                buttonWidth = parseInt(field.style.width);
            if (isNaN(buttonWidth) || (buttonWidth == null))
                buttonWidth = field.clientWidth;
            else
                fromStyle = true;
        }
        if (isNaN(parseInt(buttonWidth)))
            buttonWidth = 255;
        else if (!fromStyle) {
            if (navigator.userAgent.indexOf("MSIE") > -1)
                buttonWidth += 86;
            else if (navigator.userAgent.indexOf("Opera") > -1)
                buttonWidth += 5;
        }
        return buttonWidth;
    },

    _handleCloseButtonClick: function (args) { if (this._popupWindow) this._popupWindow.close(); },

    _handleDeleting: function (sender, args) {
        if (args.get_cancel()) return;
        var removeButton = sender._get_eventTarget();
        if (removeButton) {
            var listItem = removeButton.parentNode;
            if (listItem) {
                var listItemIndex = sender._getRowIndex(listItem);
                var fileIsNotUploadedYet = eval(listItem.getAttribute("fileIsNotUploadedYet"));
                sender._deleteUploadedFile(((fileIsNotUploadedYet == true) ? parseInt(listItem.getAttribute("fileIndex")) : listItemIndex), listItem.getAttribute("fileUniqueId"), fileIsNotUploadedYet);
            }
        }
    },

    _handleFileSelected: function (sender, args) {
        var field = args.get_fileInputField();
        if (field && field.value && (field.value.length > 0)) {
            sender._appendFile(field.value, null, null, (sender._currentIndex - 1));
            field.style.display = "none";
            sender.addFileInput();
            sender._setFilesCount(1);
        }
    },

    _handleSwfUploadFileQueueError: function (file, errorCode, message) {
        var simpleUpload = $find(this.customSettings.simpleUploadId);
        switch (errorCode) {
            case SWFUpload.QUEUE_ERROR.QUEUE_LIMIT_EXCEEDED:
                simpleUpload._showMessage(4, true);
                break;
            case SWFUpload.QUEUE_ERROR.ZERO_BYTE_FILE:
            case SWFUpload.QUEUE_ERROR.FILE_EXCEEDS_SIZE_LIMIT:
                simpleUpload._showMessage(5, true);
                break;
        }
    },

    _handleSwfUploadFileDialogComplete: function (numFilesSelected, numFilesQueued, numFilesInQueue) {
        if (numFilesSelected > 0) {
            var simpleUpload = $find(this.customSettings.simpleUploadId);
            if (numFilesSelected == numFilesQueued) simpleUpload._hideMessage();
            var numTotalFiles = numFilesInQueue;
            var stats = this.getStats();
            if (stats)
                numTotalFiles += (stats.upload_cancelled + stats.queue_errors);
            var count = 0;
            for (var i = (numTotalFiles - numFilesQueued); i < numTotalFiles; i++) {
                var file = this.getFile(i);
                simpleUpload._appendFile(file.name, file.id, null, null, null);
                count++;
            }
            simpleUpload._setFilesCount(count);
        }
    },

    _handleSwfUploadLoaded: function (swfUpload) {
        if (swfUpload) {
            var simpleUpload = $find(swfUpload.customSettings.simpleUploadId);
            if (simpleUpload)
                simpleUpload.changeLastFileInputDisabledState(false);
        }
    },

    _handleSwfUploadUploadComplete: function (file) {
        var stats = this.getStats();
        if (stats) {
            if (stats.files_queued > 0) this.startUpload();
        }
        Micajah.FileService.SimpleUpload._progressData.uploadedFilesCount++;
    },

    _handleSwfUploadUploadProgress: function (file, bytesComplete, bytesTotal) {
        var progressArea = $find(Micajah.FileService.SimpleUpload._progressAreaId);
        if (progressArea) {
            var simpleUpload = $find(this.customSettings.simpleUploadId);
            var totalFilesCount = simpleUpload.get_notUploadedFilesCount();
            var currentFilesCount = 0;
            var stats = this.getStats();
            if (stats)
                currentFilesCount = stats.successful_uploads;
            var progressData = {
                PrimaryPercent: Math.ceil((bytesComplete / file.size) * 100),
                PrimaryTotal: SWFUpload.speed.formatBytes(file.size),
                PrimaryValue: SWFUpload.speed.formatBytes(bytesComplete),
                SecondaryPercent: Math.ceil((currentFilesCount / totalFilesCount) * 100),
                SecondaryTotal: totalFilesCount,
                SecondaryValue: currentFilesCount,
                CurrentOperationText: file.name,
                TimeElapsed: Micajah.FileService._formatSeconds(file.timeElapsed),
                TimeEstimated: Micajah.FileService._formatSeconds(file.timeRemaining),
                Speed: SWFUpload.speed.formatBPS(file.currentSpeed)
            };
            Micajah.FileService.SimpleUpload._progressData.totalFilesCount = totalFilesCount;
            progressArea.show();
            progressArea.skipHandlingProgressUpdating = true;
            progressArea.update(progressData);
            progressArea.skipHandlingProgressUpdating = false;
        }
    },

    _handleSwfUploadUploadSuccess: function (file, serverData) {
        var simpleUpload = $find(this.customSettings.simpleUploadId);
        simpleUpload._addUploadedByFlashFile(serverData);
    },

    _hideMessage: function (index) {
        var validator = this._get_validator();
        validator.style.display = "none";
        validator.innerHTML = "";
    },

    _initializePopupWindow: function () {
        if (this._popupWindow) {
            this._handleCloseButtonClickDelegate = Function.createDelegate(this, this._handleCloseButtonClick);
            $get(this.get_id() + "_UploadButton").style.display = "inline";
            this.get_element().className += " suFixedFilesList";
            this._get_validator().className += " suFixedErrorArea";
        }
    },

    _initializeSwfUpload: function () {
        this._swfUpload = new SWFUpload({
            upload_url: this.get_swfUploadFileUploadUrl(),
            file_size_limit: this.get_swfUploadFileSizeLimit() + " B",
            file_types: ((this.get_allowedFileExtensions().length > 0) ? this.get_allowedFileExtensions().join(";").replace(new RegExp("\\.", "g"), "*\.") : "*.*"),
            file_types_description: "",
            file_queue_error_handler: this._handleSwfUploadFileQueueError,
            file_upload_limit: this.get_availableFilesCount(),
            file_queue_limit: this.get_availableFilesCount(),
            file_dialog_complete_handler: this._handleSwfUploadFileDialogComplete,
            swfupload_loaded_handler: this._handleSwfUploadLoaded,
            upload_progress_handler: ((this._showProgressArea && (this._popupWindow == null)) ? this._handleSwfUploadUploadProgress : null),
            upload_complete_handler: this._handleSwfUploadUploadComplete,
            upload_success_handler: this._handleSwfUploadUploadSuccess,
            button_placeholder_id: this.get_id() + "_FlashUpload",
            button_window_mode: SWFUpload.WINDOW_MODE.TRANSPARENT,
            button_width: this._getSwfUploadButtonWidth(),
            button_height: 24,
            button_text: this.get_swfUploadButtonText(),
            flash_url: this.get_swfUploadFlashUrl(),
            custom_settings: {
                simpleUploadId: this.get_id()
            },
            debug: false
        });
    },

    _initializeUploadedFiles: function () {
        if (this._enableFileService && this._showUploadedFiles) {
            var uploadedFilesField = this._get_uploadedFilesField();
            if (uploadedFilesField.value.length > 0) {
                var array1 = uploadedFilesField.value.split("|");
                var count = array1.length;
                for (var i = 0; i < count; i++) {
                    var parts = array1[i].split("*");
                    this._appendFile(parts[1], parts[0], parts[2], null);
                }
                this._setFilesCount(count);
            }
        }
    },

    _isInputWidthInPixels: function () {
        if (this._inputWidth != null)
            return (this._inputWidth.indexOf("px") > -1);
        return false;
    },

    _removeHandler: function (elementId, eventName, handler) {
        var element = $get(elementId);
        if (element && handler) $removeHandler(element, eventName, handler);
    },

    _setElementSize: function (elem, size) {
        if (elem) {
            elem.style.width = size[0];
            elem.style.height = size[1];
        }
    },

    _setFilesCount: function (increment) {
        this._filesCount += increment;
        var holder = null;
        var maxFilesCount = this.get_maxFileCount();
        if ((maxFilesCount > 0) && (increment != 0)) {
            var display = null;
            if (increment > 0) {
                if (maxFilesCount <= this._filesCount) display = "none";
            }
            else if (increment < 0) {
                if (maxFilesCount == (this._filesCount - increment)) display = "";
            }
            if (display != null) {
                holder = this._get_changeModeMessageHolder();
                if (holder) holder.style.display = display;
                var field = $get(this.get_id() + "file" + (this._currentIndex - 1));
                if (field) field.style.display = display;
            }
        }
        this._setElementSize(this._get_uploadHolder(), (((this.get_maxFileCount() > 0) ? (this.get_availableFilesCount() > 0) : true) ? ["auto", "24px"] : ["1px", "1px"]));
    },

    _showMessage: function (messageIndex, append) {
        var validator = this._get_validator();
        var message = this._messages[messageIndex];
        if (append == true) {
            if (validator.innerHTML.indexOf(message) == -1) {
                if (validator.innerHTML.length > 0) validator.innerHTML += "<br />";
                validator.innerHTML += message;
            }
        }
        else
            validator.innerHTML = message;
        validator.style.display = "block";
    },

    _synchronize: function (forced) {
        if (this._popupWindow) {
            var pb = this.isPostBack;
            if (forced && (!this.isPostBack)) this.isPostBack = true;
            var parentElementIdPrefix = this._popupWindow.get_id().replace("_PopupWindow", "");
            var parent = this._popupWindow.get_element().parentNode;
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
            if (pb != this.isPostBack) this.isPostBack = pb;
        }
    },

    addFileInputAt: function (index) {
        var count = this.get_maxFileCount();
        this.set_maxFileCount(0);
        Micajah.FileService.SimpleUpload.callBaseMethod(this, "addFileInputAt", [index]);
        this.set_maxFileCount(count);
        var listItem = this._get_lastListItem();
        var field = this.getFileInputFrom(listItem);
        field.tabIndex = this.get_inputTabIndex();
        field.tabStop = (field.tabIndex > -1);
        if (this.get_inputWidth() != null)
            field.style.width = this.get_inputWidth();
        var holder = this._get_uploadHolder();
        holder.appendChild(field);
        listItem.parentNode.removeChild(listItem);
    },

    cancelUpload: function () {
        if (this._enableFileService) {
            this._get_deletedFilesField().value = "";
            var uploadedFilesField = this._get_uploadedFilesField();
            uploadedFilesField.value = uploadedFilesField.defaultValue;
        }
        var fields = this.getFileInputs();
        for (i = 0; i < (fields.length - 1); i++) {
            var inp = fields[i];
            $clearHandlers(inp);
            inp.parentNode.removeChild(inp);
        }
        if (this._swfUpload) {
            var stats = this._swfUpload.getStats();
            if (stats) {
                for (var i = 0; i < (stats.files_queued + stats.upload_cancelled); i++) {
                    var file = this._swfUpload.getFile(i);
                    if (file) {
                        if (file.filestatus == SWFUpload.FILE_STATUS.QUEUED)
                            this._swfUpload.cancelUpload(file.id);
                    }
                }
            }
        }
    },

    changeMode: function (mode, hideMessage) {
        if (!this._enableFileService) return;
        if (isNaN(parseInt(mode))) mode = this.get_currentMode();
        if (mode == 1) {
            if (this.get_flashIsEnabled() == false) mode = 0;
        }
        var currentModeField = this._get_currentModeField();
        currentModeField.value = mode;
        if (mode == 1) {
            this._setElementSize(this._get_swfUploadHolder(), ["auto", "auto"]);
            if (this._swfUpload == null) {
                this.changeLastFileInputDisabledState(true);
                this._initializeSwfUpload();
            }
            else {
                if (this._swfUpload.getStats() == null)
                    this.changeLastFileInputDisabledState(true);
                this._changeModeMessage();
            }
            if (this._swfUpload) {
                var filesCount = this.get_availableFilesCount();
                this._swfUpload.setFileQueueLimit(filesCount);
                this._swfUpload.setFileUploadLimit(filesCount);
            }
        }
        else {
            this._setElementSize(this._get_swfUploadHolder(), ["1px", "1px"]);
            this.changeLastFileInputDisabledState(false);
        }       
    },

    changeLastFileInputDisabledState: function (state) {
        var controls = this.getFileInputs();
        var controlsCount = controls.length;
        if (controlsCount > 0)
            controls[controlsCount - 1].disabled = state;
        this._changeModeMessage();
    },

    dispose: function () {
        this._handleCloseButtonClickDelegate = null;
        this.remove_deleting(this._handleDeleting);
        this.remove_fileSelected(this._handleFileSelected);
        Array.remove(Micajah.FileService.SimpleUpload._controls, this.get_id());
        Micajah.FileService.SimpleUpload.callBaseMethod(this, "dispose");
    },

    getFileInputs: function () {
        var fields = [];
        var inputs = this.get_element().parentNode.getElementsByTagName("INPUT");
        for (var i = 0; i < inputs.length; i++) {
            var inp = inputs[i];
            if (inp.type == "file") fields[fields.length] = inp;
        }
        return fields;
    },

    initialize: function () {
        Micajah.FileService.SimpleUpload.callBaseMethod(this, "initialize");
        this._popupWindow = this._get_popupWindow();
        this._initializePopupWindow();
        this._synchronize();
        if (this._enableFileService) this._initializeUploadedFiles();
        this.add_deleting(this._handleDeleting);
        this.add_fileSelected(this._handleFileSelected);
        this.changeLastFileInputDisabledState(true);
        if ((navigator.userAgent.indexOf("MSIE") == -1) || (this.get_currentMode() == 0) || this._isInputWidthInPixels())
            this.changeMode();
        Array.add(Micajah.FileService.SimpleUpload._controls, this.get_id());
    },

    isExtensionValid: function (fileExtension) {
        if (this.get_allowedFileExtensions().length == 0) return true;
        return Micajah.FileService.SimpleUpload.callBaseMethod(this, "isExtensionValid", [fileExtension]);
    },

    isUploadControl: function (controlId) {
        for (var i = 0; i < this._uploadControlsUniqueId.length; i++) {
            if (controlId == this._uploadControlsUniqueId[i])
                return true;
        }
        return false;
    },

    isValid: function () {
        this._hideMessage();
        var valid = true;
        var fields = this.getFileInputs();
        for (var i = 0; i < (fields.length - 1); i++) {
            if (!this.isExtensionValid(fields[i].value)) {
                valid = false;
                break;
            }
        }
        if (!valid) this._showMessage(0);
        return valid;
    }
}

Micajah.FileService.SimpleUpload._controls = new Array();
Micajah.FileService.SimpleUpload._progressAreaId = null;
Micajah.FileService.SimpleUpload._progressData = {
    totalFilesCount: 0,
    uploadedFilesCount: 0
};

Micajah.FileService.SimpleUpload._initializeControls = function (sender, args) {
    if (args.get_isPartialLoad())
        return;
    for (var i = 0; i < Micajah.FileService.SimpleUpload._controls.length; i++) {
        var ctl = $find(Micajah.FileService.SimpleUpload._controls[i]);
        if (ctl != null) {
            if ((ctl.get_currentMode() == 1) && (!ctl._isInputWidthInPixels()))
                ctl.changeMode();
        }
    }
}

if (navigator.userAgent.indexOf("MSIE") > -1) {
    Sys.Application.add_load(Micajah.FileService.SimpleUpload._initializeControls);
}

Micajah.FileService.SimpleUpload._handleProgressAreaProgressUpdating = function (sender, args) {
    if (sender.skipHandlingProgressUpdating == true) return;
    var data = args.get_progressData();
    if (Micajah.FileService.SimpleUpload._progressData.totalFilesCount > 0)
        data.SecondaryTotal = Micajah.FileService.SimpleUpload._progressData.totalFilesCount;
    var totalFilesCount = parseInt(data.SecondaryTotal);
    if (isNaN(totalFilesCount)) totalFilesCount = 0;
    var currentFilesCount = parseInt(data.SecondaryValue);
    if (isNaN(currentFilesCount)) currentFilesCount = 0;
    currentFilesCount += Micajah.FileService.SimpleUpload._progressData.uploadedFilesCount;
    data.SecondaryValue = currentFilesCount;
    data.SecondaryPercent = Math.ceil((currentFilesCount / totalFilesCount) * 100);
}

Micajah.FileService.SimpleUpload._startUpload = function () {
    if (typeof (Micajah.FileService.SimpleUpload._controls) != "undefined") {
        for (var i = 0; i < Micajah.FileService.SimpleUpload._controls.length; i++) {
            var ctl = $find(Micajah.FileService.SimpleUpload._controls[i]);
            if (ctl != null) {
                if (ctl.isUploadControl(Micajah.FileService._eventTarget)) {
                    if (ctl._swfUpload != null) {
                        var stats = ctl._swfUpload.getStats();
                        if (stats) {
                            if (stats.files_queued > 0)
                                ctl._swfUpload.startUpload();
                        }
                    }
                }
                else
                    ctl.cancelUpload();
            }
        }
    }
}

Micajah.FileService.SimpleUpload._uploadCompleted = function () {
    if (typeof (Micajah.FileService.SimpleUpload._controls) != "undefined") {
        for (var i = 0; i < Micajah.FileService.SimpleUpload._controls.length; i++) {
            var ctl = $find(Micajah.FileService.SimpleUpload._controls[i]);
            if ((ctl != null) && (ctl._swfUpload != null)) {
                var stats = ctl._swfUpload.getStats();
                if (stats) {
                    if (stats.files_queued > 0)
                        return false;
                }
            }
        }
    }
    return true;
}

Micajah.FileService.SimpleUpload._validateControls = function () {
    if (typeof (Page_IsValid) == "boolean") {
        if (Page_IsValid == false)
            return 1;
    }
    if (typeof (Micajah.FileService.SimpleUpload._controls) != "undefined") {
        var count = Micajah.FileService.SimpleUpload._controls.length;
        for (var i = 0; i < count; i++) {
            var ctl = $find(Micajah.FileService.SimpleUpload._controls[i]);
            if (ctl != null) {
                if (ctl.isUploadControl(Micajah.FileService._eventTarget)) {
                    if (ctl.isValid() == false)
                        return 2;
                }
                else
                    count--;
            }
        }
        if (count == 0)
            return 1;
    }
    return 0;
}

Micajah.FileService.SimpleUpload.registerClass("Micajah.FileService.SimpleUpload", Telerik.Web.UI.RadUpload);

Micajah.FileService._originalDoPostBack = __doPostBack;
Micajah.FileService._eventTarget = "";
Micajah.FileService._eventArgument = "";
Micajah.FileService._tryDoPostBackTimer = null;

Micajah.FileService._formatSeconds = function (seconds) {
    var h = 0;
    var m = 0;
    var s = Math.ceil(seconds);
    if (seconds > 60) {
        m = Math.floor(seconds / 60);
        if (m > 60) h = Math.floor(m / 60);
    }
    if (h < 10) h = "0" + h;
    if (m < 10) m = "0" + m;
    if (s < 10) s = "0" + s;
    return h + ":" + m + ":" + s + "s";
}

Micajah.FileService._tryDoPostBack = function () {
    if (Micajah.FileService.SimpleUpload._uploadCompleted() == true) {
        window.clearInterval(Micajah.FileService._tryDoPostBackTimer);
        if (typeof (theForm) != "undefined") {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            if (prm != null) {
                if (prm._onsubmit != null)
                    theForm.onsubmit = prm._onsubmit;
            }
        }
        Micajah.FileService._tryDoPostBackTimer = null;
        Micajah.FileService._originalDoPostBack(Micajah.FileService._eventTarget, Micajah.FileService._eventArgument);
    }
}

__doPostBack = function (eventTarget, eventArgument) {
    if (Micajah.FileService._tryDoPostBackTimer == null) {
        Micajah.FileService._eventTarget = eventTarget;
        Micajah.FileService._eventArgument = eventArgument;
        var code = Micajah.FileService.SimpleUpload._validateControls();
        switch (code) {
            case 0:
                Micajah.FileService.SimpleUpload._startUpload();
                Micajah.FileService._tryDoPostBackTimer = window.setInterval(Micajah.FileService._tryDoPostBack, 500);
                break;
            case 1:
                Micajah.FileService._originalDoPostBack(Micajah.FileService._eventTarget, Micajah.FileService._eventArgument);
                break;
        }
    }
}
