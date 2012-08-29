function FileList_ToolTipBeforeShow(sender, eventArgs) {
    var content = "";
    var fileUrl = "#";
    var fileName = "";
    var fileInfo = sender.get_serverValue();
    var targetElement = sender.get_targetControl();
    var isThumbnailsList = (fileInfo.length > 0);
    content = ((isThumbnailsList == true)
        ? '<div style="width: 250px" class="flToolTip"><a class="flFileName" href="FILE_URL" target="_blank">FILE_NAME</a><span class="flFileInfo">FILE_INFO</span>DELETE_LINK</div>'
        : '<div style="width: 600px; height: 500px" class="flToolTip"><a class="flFileName" href="FILE_URL" target="_blank"><img style="border-right-width: 0px; border-top-width: 0px; border-bottom-width: 0px; border-left-width: 0px" alt="FILE_NAME" src="FILE_URL&Width=600&Height=500&Align=1"></a></div>');
    if (targetElement != null) {
        if (isThumbnailsList == true) {
            var links = targetElement.getElementsByTagName("A");
            var deleteLink = "";
            for (var i = 0; i < links.length; i++) {
                var link = links[i];
                if (link.id.indexOf("_DeleteLink") > -1) {
                    if (typeof (link.outerHTML) == "undefined") {
                        var attrs = link.attributes;
                        var deleteLink = "<" + link.tagName;
                        for (var i = 0; i < attrs.length; i++) {
                            deleteLink += " " + attrs[i].name + "=\"" + attrs[i].value.replace(new RegExp("\"", "ig"), "&quot;") + "\"";
                        }
                        deleteLink += ">" + link.innerHTML + "</" + link.tagName + ">";
                    }
                    else
                        deleteLink = link.outerHTML;
                    deleteLink = deleteLink.replace("none", "auto");
                }
                else
                    fileUrl = links[i].href;
            }
            content = content.replace("DELETE_LINK", deleteLink);
            var parts = fileInfo.split("|");
            fileInfo = parts[0];
            if (parts.length > 1) fileName = parts[1];
            content = content.replace("FILE_INFO", fileInfo);
        }
        else {
            fileUrl = targetElement.href;
            fileName = targetElement.innerHTML;
        }
    }
    content = content.replace(new RegExp("FILE_URL", "g"), fileUrl);
    content = content.replace("FILE_NAME", fileName);
    sender.set_content(content);
}
