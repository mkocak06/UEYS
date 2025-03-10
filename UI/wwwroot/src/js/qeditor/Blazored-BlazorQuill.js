(function () {
    window.QuillFunctions = {        
        createQuill: function (
            quillElement, toolBar, readOnly,
            placeholder, theme, debugLevel) {  

            Quill.register('modules/blotFormatter', QuillBlotFormatter.default);
            Quill.register('modules/imageCompressor', imageCompressor);

            var options = {
                debug: debugLevel,
                modules: {
                    toolbar: toolBar,
                    blotFormatter: {},
                    imageCompressor: {
                        quality: 0.4,
                        maxWidth: 750, // default
                        maxHeight: 750, // default
                        imageType: 'image/jpeg'
                    }
                },
                placeholder: placeholder,
                readOnly: readOnly,
                theme: theme
            };

            new Quill(quillElement, options);
            document.getElementsByClassName("ql-editor")[0].setAttribute("tabindex", 2);
        },
        getQuillContent: function(quillElement) {
            return JSON.stringify(quillElement.__quill.getContents());
        },
        getQuillText: function(quillElement) {
            return quillElement.__quill.getText();
        },
        getQuillHTML: function(quillElement) {
            return quillElement.__quill.root.innerHTML;
        },
        loadQuillContent: function(quillElement, quillContent) {
            content = JSON.parse(quillContent);
            return quillElement.__quill.setContents(content, 'api');
        },
        loadQuillHTMLContent: function (quillElement, quillHTMLContent) {
            return quillElement.__quill.root.innerHTML = quillHTMLContent;
        },
        enableQuillEditor: function (quillElement, mode) {
            quillElement.__quill.enable(mode);
        },
        insertQuillImage: function (quillElement, imageURL) {
            var Delta = Quill.import('delta');
            editorIndex = 0;

            if (quillElement.__quill.getSelection() !== null) {
                editorIndex = quillElement.__quill.getSelection().index;
            }

            return quillElement.__quill.updateContents(
                new Delta()
                    .retain(editorIndex)
                    .insert({ image: imageURL },
                        { alt: imageURL }));
        },
        registerQuillEvent: function (quillElement, dotnetHelper, eventName) {
            if (!quillElement[`__quill_event_${eventName}`]) {
                quillElement[`__quill_event_${eventName}`] = function () {
                    dotnetHelper.invokeMethodAsync("QuillEventCallbackCaller", eventName, [...arguments])
                }
                quillElement.__quill.on(eventName, quillElement[`__quill_event_${eventName}`]);
            }
        },
        unregisterQuillEvent: function (quillElement, eventName) {
            if (quillElement[`__quill_event_${eventName}`]) {
                quillElement.__quill.off(eventName, quillElement[`__quill_event_${eventName}`]);
            }
        }
    };
})();