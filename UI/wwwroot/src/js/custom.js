
function CallJQueryFunctions() {
    $('[data-toggle="tooltip"]').tooltip();
    $('[data-toggle="tooltip"]').on('click', function () {
        $(this).tooltip('hide');
    });
    $('[data-toggle="dropdown"]').dropdown();
    $('.dropdown-toggle').dropdown();
    $(".scroll").each((k, v) => new PerfectScrollbar(v));
    //KTLayoutAsideMenu.init('kt_aside_menu');
    //KTLayoutAsideToggle.init('kt_body');
    // Init Desktop & Mobile Headers
    if (typeof KTLayoutHeader !== 'undefined') {
        KTLayoutHeader.init('kt_header', 'kt_header_mobile');
    }

    // Init Header Topbar For Mobile Mode
    if (typeof KTLayoutHeaderTopbar !== 'undefined') {
        KTLayoutHeaderTopbar.init('kt_header_mobile_topbar_toggle');
    }

    // Init Brand Panel For Logo
    if (typeof KTLayoutBrand !== 'undefined') {
        KTLayoutBrand.init('kt_brand');
    }

    // Init Aside
    if (typeof KTLayoutAside !== 'undefined') {
        KTLayoutAside.init('kt_aside');
    }

    // Init Aside Menu Toggle
    if (typeof KTLayoutAsideToggle !== 'undefined') {
        KTLayoutAsideToggle.init('kt_aside_toggle');
    }

    // Init Aside Menu
    if (typeof KTLayoutAsideMenu !== 'undefined') {
        KTLayoutAsideMenu.init('kt_aside_menu');
    }

    // Init Subheader
    if (typeof KTLayoutSubheader !== 'undefined') {
        KTLayoutSubheader.init('kt_subheader');
    }

    // Init Content
    if (typeof KTLayoutContent !== 'undefined') {
        KTLayoutContent.init('kt_content');
    }

    // Init Footer
    if (typeof KTLayoutFooter !== 'undefined') {
        KTLayoutFooter.init('kt_footer');
    }


    //////////////////////////////////////////////
    // Layout Extended Partials(optional to use)//
    //////////////////////////////////////////////

    // Init Sticky Card
    if (typeof KTLayoutStickyCard !== 'undefined') {
        KTLayoutStickyCard.init('kt_page_sticky_card');
    }

    // Init Stretched Card
    if (typeof KTLayoutStretchedCard !== 'undefined') {
        KTLayoutStretchedCard.init('kt_page_stretched_card');
    }

    // Init Code Highlighter & Preview Blocks(used to demonstrate the theme features)
    if (typeof KTLayoutExamples !== 'undefined') {
        KTLayoutExamples.init();
    }

    // Init Demo Selection Panel
    if (typeof KTLayoutDemoPanel !== 'undefined') {
        KTLayoutDemoPanel.init('kt_demo_panel');
    }

    // Init Chat App(quick modal chat)
    if (typeof KTLayoutChat !== 'undefined') {
        KTLayoutChat.init('kt_chat_modal');
    }

    // Init Quick Actions Offcanvas Panel
    if (typeof KTLayoutQuickActions !== 'undefined') {
        KTLayoutQuickActions.init('kt_quick_actions');
    }

    // Init Quick Notifications Offcanvas Panel
    if (typeof KTLayoutQuickNotifications !== 'undefined') {
        KTLayoutQuickNotifications.init('kt_quick_notifications');
    }

    // Init Quick Offcanvas Panel
    if (typeof KTLayoutQuickPanel !== 'undefined') {
        KTLayoutQuickPanel.init('kt_quick_panel');
    }

    // Init Quick User Panel
    if (typeof KTLayoutQuickUser !== 'undefined') {
        KTLayoutQuickUser.init('kt_quick_user');
    }

    // Init Quick Search Panel
    if (typeof KTLayoutQuickSearch !== 'undefined') {
        KTLayoutQuickSearch.init('kt_quick_search');
    }

    // Init Quick Cart Panel
    if (typeof KTLayoutQuickCartPanel !== 'undefined') {
        KTLayoutQuickCartPanel.init('kt_quick_cart');
    }

    // Init Search For Quick Search Dropdown
    if (typeof KTLayoutSearch !== 'undefined') {
        KTLayoutSearch().init('kt_quick_search_dropdown');
    }

    // Init Search For Quick Search Offcanvas Panel
    if (typeof KTLayoutSearchOffcanvas !== 'undefined') {
        KTLayoutSearchOffcanvas().init('kt_quick_search_offcanvas');
    }
    initSwitch();

}
function initPopOver() {
    $('[data-toggle="popover"]').each(function () {
        var $this = $(this);
        $this.popover({
            trigger: 'hover',
            container: $this
        })
    });
}
function initQuickPanel() {
    // Init Quick Offcanvas Panel
    if (typeof KTLayoutQuickPanel !== 'undefined') {
        KTLayoutQuickPanel.init('kt_quick_panel');
    }
}
function initStickyPanel() {
    // Init Quick Offcanvas Panel
    if (typeof KTLayoutStickyCard !== 'undefined') {
        KTLayoutStickyCard.init('kt_page_sticky_card');
    }
}
function initQuickPanelForSupport() {
    // Init Quick Offcanvas Panel
    console.log("init1:");

    if (typeof KTLayoutQuickPanel !== 'undefined') {
        KTLayoutQuickPanel.init('kt_quick_panel_support');
    }
}
function initStatsWidget2(statsDataValues, statsDataNames) {
    // Init Stats Widgets
    if (typeof KTWidgets !== 'undefined') {
        KTWidgets.init(statsDataValues, statsDataNames);
    }
}

function initTooltip() {
    $('[data-toggle="tooltip"]').tooltip();
}
function initSwitch() {
    $('[data-switch=true]').bootstrapSwitch();
}

function clearDatePicker(id) {
    $('#' + id).val("").datepicker("update");
}

function openModal(element) {
    $('#' + element).modal("show")
}

function closeModal(element) {
    $('#' + element).modal("hide")
}

function initDropdown(element) {
    $('[data-toggle="dropdown"]').dropdown();
}

function BlazorDownloadFile(filename, contentType, data) {

    // Create the URL
    const file = new File([data], filename, { type: contentType });
    const exportUrl = URL.createObjectURL(file);

    // Create the <a> element and click on it
    const a = document.createElement("a");
    document.body.appendChild(a);
    a.href = exportUrl;
    a.download = filename;
    a.target = "_self";
    a.click();

    // We don't need to keep the url, let's release the memory
    // On Safari it seems you need to comment this line... (please let me know if you know why)
    URL.revokeObjectURL(exportUrl);
    a.remove();
}

async function getSvgIcon(name) {
    return await new Promise(resolve => {
        fetch('/src/svg.json')
            .then((result) => {
                result.json().then(function (data) {
                    let searchResult = data.find(x => x.name === name)
                    resolve(searchResult.content)
                })
            })
            .catch(err => console.log(err));
    });
}

function blockPage(text) {
    KTApp.blockPage({
        overlayColor: '#000000',
        state: 'primary',
        message: text
    });
}

function unblockPage() {
    KTApp.unblockPage();
}

function blockElement(element, message) {
    if (!message) {
        KTApp.block(element, {});
    } else {
        KTApp.block(element, {
            overlayColor: '#000000',
            state: 'primary',
            message: message
        });
    }
}
function unblockElement(element) {
    KTApp.unblock(element);
}

function initSelect2() {
    $.each($('.select2'), function (i, val) {
        $(val).select2();
    });
}

function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}

window.FlatpickrHelpers = {
    instances: [],

    findInstance(id) {
        return this.instances.find(x => x.id === id);
    },

    destroy(id) {
        let instance = this.findInstance(id);
        if (instance && instance.object) {
            instance.object.destroy();
            this.instances = this.instances.filter(x => x.id !== id)
        }
    },

    clear(id) {
        let instance = this.findInstance(id);
        if (instance && instance.object) {
            //instance.object.clear();
            instance.object.setDate(null, false);
        }
    },

    close(id) {
        let instance = this.findInstance(id);
        if (instance && instance.object) {
            instance.object.close();
        }
    },

    render(id, options, reference) {
        let optionsParsed = this.getOptionsParsed(options);
        optionsParsed = {
            onChange: function (selectedDates, dateStr, instance) {
                console.log(document.getElementById(id)._flatpickr.selectedDates)
                reference.invokeMethodAsync("OnChangeDate", FlatpickrHelpers.findInstance(id).object.selectedDates);
            },
            ...optionsParsed
        }
        console.log(optionsParsed);
        let calendar = flatpickr(document.getElementById(id), optionsParsed);
        this.instances.push({ id: id, object: calendar })
    },

    setDate(id, date) {
        let instance = this.findInstance(id);
        instance.object.setDate(date, false)
    },

    getOptionsParsed(options) {
        return JSON.parse(options, (key, value) => {
            if (value === null) {
                return;
            }
            return value;
        }
        );
    }
}
window.datePickerFuctions = {
    InitDatePickerwithSelect: function (element, formatDate, minDate, maxDate, objectRef) {
        $(element).datepicker('destroy');
        $(element).datepicker({
            showOtherMonths: true,
            selectOtherMonths: true,
            changeMonth: true,
            changeYear: true,
            dateFormat: formatDate,
            language: 'tr',
            startDate: minDate == null ? null : new Date(minDate),
            endDate: maxDate == null ? null : new Date(maxDate),
            onSelect: function (date) {
                var myElement = $(this)[0];
                var event = new Event('change');
                myElement.dispatchEvent(event);
            }
        }).on('changeDate', function (ev) {
            $(this).datepicker('hide');
            objectRef.invokeMethodAsync("OnChangeDate", ev.date.toJSON());
        })
    },
    SetDate: function (element, date) {
        $(element).datepicker('setDate', date);
    },
    SetMinMaxDate: function (element, minDate, maxDate) {
        var min = minDate == null ? null : new Date(minDate);
        var max = maxDate == null ? null : new Date(maxDate);
        $(element).datepicker('option', 'startDate', min);
        $(element).datepicker('option', 'endDate', max);
    },
    InitInlineDatePicker: function (element, date, objectRef) {
        $(element).datepicker('destroy');
        $(element).datepicker({
            showOtherMonths: true,
            selectOtherMonths: true,
            changeMonth: true,
            changeYear: true,
            defaultViewDate: date,
            todayHighlight: true,
            templates: {
                leftArrow: '<i class="la la-angle-left"></i>',
                rightArrow: '<i class="la la-angle-right"></i>'
            },
            language: 'tr',
            onSelect: function (date) {
                var myElement = $(this)[0];
                var event = new Event('change');
                myElement.dispatchEvent(event);
            }
        }).on('changeDate', function (ev) {
            if (ev.date) {
                objectRef.invokeMethodAsync("OnChangeDate", ev.date.toJSON());
            }
        })
    },
}

window.dateRangePickerFunctions = {
    initDateRangePicker: function (element, startDate, endDate, formatDate, objectRef) {
        console.log(startDate, endDate)
        let sDate = new Date(startDate);
        let eDate = new Date(endDate);
        $(element).daterangepicker('destroy');
        $(element).daterangepicker({
            buttonClasses: ' btn',
            applyClass: 'btn-primary',
            cancelClass: 'btn-secondary',
            showDropdowns: true,
            locale: {
                format: "DD/MM/YYYY",
                applyLabel: "Uygula",
                cancelLabel: "İptal",
                fromLabel: "den",
                toLabel: "a",
                customRangeLabel: "Özel",
                daysOfWeek: [
                    "Paz",
                    "Pts",
                    "Sal",
                    "Çar",
                    "Per",
                    "Cum",
                    "Cts"
                ],
                monthNames: [
                    "Ocak",
                    "Şubat",
                    "Mart",
                    "Nisan",
                    "Mayıs",
                    "Haziran",
                    "Temmuz",
                    "Ağustos",
                    "Eylül",
                    "Ekim",
                    "Kasım",
                    "Aralık"
                ],
                firstDay: 1
            }
        });
        if (startDate !== null) {
            $(element).data('daterangepicker').setStartDate(sDate);
        }
        if (endDate !== null) {
            $(element).data('daterangepicker').setEndDate(eDate);
        }
        $(element).on('apply.daterangepicker', function (ev, picker) {
            objectRef.invokeMethodAsync("OnChangeDate", [picker.startDate.toJSON(), picker.endDate.toJSON()]);
            $(element + ' .form-control').val(picker.startDate.format('YYYY-MM-DD') + ' / ' + picker.endDate.format('YYYY-MM-DD'));
        });
    },
    setDate: function (element, date) {
        $(element).datepicker('setDate', date);
    }
}

function initializeQuillListener(quillDiv) {
    let quillEdt = quillDiv.querySelector('.ql-container');
    quillDiv.attr('tabindex', '2');
    if (quillDiv != null) {
        quillEdt.on('text-change', function (delta, oldDelta, source) {
            if (source == 'api') {
                console.log("An API call triggered this change.");
            } else if (source == 'user') {
                console.log("A user action triggered this change.");
            }
        });
    }
}

var tagify = null;

function initializeTagify(dotnetHelper, input, taglist) {
    if (input == null) {
        input = document.getElementsByClassName('tagify');
    }
    //console.log(taglist);
    // init Tagify script on the above inputs
    tagify = new Tagify(input, {
        //whitelist: [{ value: "ali", id: 1 }, { value: "veli", id: 2 }],
        whitelist: taglist,
        editTags: false,
        keepInvalidTags: true,
        transformTag: function (tagData) {
            tagData.class = 'tagify__tag tagify__tag--primary';
            //console.log(tagData);
        }
    })

    // Chainable event listeners
    tagify.on('add', onAddTag)
        .on('remove', onRemoveTag)
        .on('input', onInput)
        .on('edit', onTagEdit)
        .on('invalid', onInvalidTag)
        .on('click', onTagClick)
        .on('dropdown:show', onDropdownShow)
        .on('dropdown:hide', onDropdownHide)

    // tag added callback
    function onAddTag(e) {
        //console.log(e.detail);
        let id = e.detail.data.id == null ? 0 : e.detail.data.id;
        dotnetHelper.invokeMethodAsync("HandleTagifyEvents", "OnAddTag", e.detail.data.value, id);
    }

    // tag remvoed callback
    function onRemoveTag(e) {
        //console.log(e.detail);
        let id = e.detail.data.id == null ? 0 : e.detail.data.id;
        dotnetHelper.invokeMethodAsync("HandleTagifyEvents", "OnRemoveTag", e.detail.data.value, e.detail.data.id);
    }

    // on character(s) added/removed (user is typing/deleting)
    function onInput(e) {
        //console.log(e.detail);
        //console.log("onInput: ", e.detail);
    }

    function onTagEdit(e) {
        //dotnetHelper.invokeMethodAsync("HandleTagifyEvents", "OnEditTag", e.detail.data.value, e.detail.data.id);
    }

    // invalid tag added callback
    function onInvalidTag(e) {
        console.log("onInvalidTag: ", e.detail);
    }

    // invalid tag added callback
    function onTagClick(e) {
    }

    function onDropdownShow(e) {
    }

    function onDropdownHide(e) {
    }
}

function loadTags() {
    tagify.destroy();
}

function initializeMetaHeadTooltip() {
    $('#meta-head [data-toggle="tooltip"]').tooltip();
}

//Language Selector
window.appCulture = {
    get: () => window.localStorage['AppLanguage'],
    set: (value) => window.localStorage['AppLanguage'] = value
};


function initializeTree(dotnetHelper, element, data) {
    let parsedData = JSON.parse(data);
    if ($.jstree.reference(element)) {
        $.jstree.reference(element).destroy();
    }

    $(element).jstree({
        "plugins": ["wholerow", "checkbox", "types"],
        "core": {
            "themes": {
                dots: false,
                icons: true,
                "responsive": true,
                "variant": "large"
            },
            "data": parsedData
        },
        "types": {
            "default": {
                "icon": "fas fa-list text-primary"
            }
        },
    });
    $(element).on('select_node.jstree', function (node, action) {
        dotnetHelper.invokeMethodAsync("OnCategorySelectionChanged", action.selected.map(x => parseInt(x)));
    }).on('deselect_node.jstree', function (node, action) {
        var selected = $.jstree.reference(element).get_selected();
        dotnetHelper.invokeMethodAsync("OnCategorySelectionChanged", selected.map(x => parseInt(x)));
    });
    //console.log($(element).jstree(true).get_json('#', { flat: true }));
}

function createNode(element, newNodeName) {
    $(element).jstree(true).create_node("#", newNodeName);
}

function setNodeId(element, oldId, newId) {
    let node = $(element).jstree(true).get_node(oldId);
    $(element).jstree(true).set_id(node, newId);
}

window.SweetAlert = {
    basicAlert: function (message) {
        if (message != null && message.length > 0)
            Swal.fire(message)
    },
    iconAlert: function (icon, title, message) {
        if (message != null && message.length > 0) {
            Swal.fire({
                icon: icon,
                title: title,
                text: message,
            });
        }
    },
    confirmAlert: function (title, message, icon, showCancelButton, confirmButtonText, cancelButtonText) {
        return new Promise(resolve => {
            Swal.fire({
                title: title,
                text: message,
                icon: icon,
                showCancelButton: showCancelButton,
                confirmButtonColor: '#008386',
                cancelButtonColor: '#ed886d',
                confirmButtonText: confirmButtonText,
                cancelButtonText: cancelButtonText
            }).then((result) => {
                resolve(result.isConfirmed)
            })
        })
    },
    toastAlert: function (icon, message) {
        const Toast = Swal.mixin({
            toast: true,
            position: 'bottom-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true,
            didOpen: (toast) => {
                toast.addEventListener('mouseenter', Swal.stopTimer)
                toast.addEventListener('mouseleave', Swal.resumeTimer)
            }
        });
        Toast.fire({
            icon: icon,
            title: message
        });
    },
    inputAlert: async function (title, orgName, typeList) {
        console.log(typeList);
        let optionsHTML = '';
        typeList.forEach(function (v) {
            optionsHTML += `<option value="${v.value}">${v.name}</option>`
        })
        return new Promise(resolve => {
            Swal.fire({
                title: title,
                html:
                    '<div class="form-group">' +
                    '<label>Kurum Adı</label>\n' +
                    `<input id="organization-name" class="form-control valid" value="${orgName}">` +
                    '</div>' +
                    '<div class="form-group">' +
                    '<label>Kurum Kısaltması</label>\n' +
                    '<input id="organization-abbreviation" class="form-control valid">' +
                    '</div>' +
                    '<div class="form-group">' +
                    '<label>Kurum Türü</label>\n' +
                    '<select id="organization-type" class="form-control valid">' +
                    optionsHTML +
                    '</select>' +
                    '</div>',
                focusConfirm: false,
                heightAuto: false,
                preConfirm: () => {
                    let org = {
                        name: document.getElementById('organization-name').value,
                        abbreviation: document.getElementById('organization-abbreviation').value,
                        statisticOrganizationType: document.getElementById('organization-type').value
                    }
                    if (org.name.length === 0) {
                        Swal.showValidationMessage("Lütfen kurum ismini giriniz!")
                        return false;
                    }
                    if (org.abbreviation.length === 0) {
                        Swal.showValidationMessage("Lütfen kurum kısaltmasını giriniz!")
                        return false;
                    }
                    return JSON.stringify(org);
                }
            }).then((result) => {
                resolve(result.value)
            })
        })
    }
}

function initializeCard(element, dotnetHelper) {
    var card = new KTCard(element);

    // Toggle event handlers
    card.on('beforeCollapse', function (card) {
        dotnetHelper.invokeMethodAsync("OnBeforeCollapse");
    });

    card.on('afterCollapse', function (card) {
        dotnetHelper.invokeMethodAsync("OnAfterCollapse");
    });

    card.on('beforeExpand', function (card) {
        dotnetHelper.invokeMethodAsync("OnBeforeExpand");
    });

    card.on('afterExpand', function (card) {
        dotnetHelper.invokeMethodAsync("OnAfterExpand");
    });

    // Remove event handlers
    card.on('beforeRemove', function (card) {
        dotnetHelper.invokeMethodAsync("OnBeforeRemove");
    });

    card.on('afterRemove', function (card) {
        dotnetHelper.invokeMethodAsync("OnAfterRemove");
    });

    // Reload event handlers
    card.on('reload', function (card) {
        dotnetHelper.invokeMethodAsync("OnReload");
    });
}

function clearInput(name) {
    $("input[name='" + name + "']").val('');
}
function clearSelectInput(name) {
    $("select[name='" + name + "']").prop('selectedIndex', -1);
}


function HideAdvancedSearchAsync() {
    if (typeof KTLayoutQuickUser !== 'undefined') {
        KTLayoutQuickPanel.getObject().hide()
    }
}

function ShowAdvancedSearchAsync() {
    // Init Calendar
    if (typeof KTLayoutQuickPanel !== 'undefined') {
        KTLayoutQuickPanel.getObject().show()
    }
}

function ShowSupportLayout() {
    console.log("init:");

    // Init Calendar
    if (typeof KTLayoutQuickPanel !== 'undefined') {
        KTLayoutQuickPanel.getObject().show()
    }
}
function HideSupportLayout() {
    if (typeof KTLayoutQuickPanel !== 'undefined') {
        KTLayoutQuickPanel.getObject().hide()
    }
}

function setLanguage(lang) {
    document.documentElement.setAttribute('lang', lang)
}
window.mask = (id, mask) => {
    $("#" + id).inputmask("mask", {
        "mask": mask
    });
};
window.unmaskedValue = (id) => {
    return $('#' + id).inputmask('unmaskedvalue');
};
window.initializeSelectPicker = function () {
    $('.selectpicker').selectpicker({
        liveSearch: true,
        noneResultsText: 'Sonuç bulunamadı: {0}',
        deselectAllText: 'Tümünü Kaldır',
        selectAllText: 'Tümünü Seç',
        countSelectedText: '{0} öğe seçildi.',
    });
}

function ToggleFullScreenElement(id) {
    var element = document.getElementById(id);
    if (element.requestFullscreen) {
        element.requestFullscreen();
    } else if (element.webkitRequestFullscreen) { /* Safari */
        element.webkitRequestFullscreen();
    } else if (element.msRequestFullscreen) { /* IE11 */
        element.msRequestFullscreen();
    }
}
function scrollToValidationSummary() {
    var validationSummary = document.getElementById("validationSummary");

    if (validationSummary) {
        validationSummary.scrollIntoView({ behavior: 'smooth' });
    }
}
function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}
var timeline;
function timeLine(id, dataModel) {
    var container = document.getElementById(id);
    // Create a DataSet (allows two way data-binding)
    var items = new vis.DataSet(dataModel);

    // Configuration for the Timeline
    var options = {

        locales: {
            // create a new locale (text strings should be replaced with localized strings)
            mylocale: {
                current: 'current',
                time: 'time',
                deleteSelected: 'Delete selected'
            }
        },

        // use the new locale
        locale: 'mylocale',
        //max: "2022-11-21", -> Set a maximum Date for the visible range
        //min: "2022-12-21"  -> Set a minimum Date for the visible range
    };

    // Create a Timeline
    timeline = new vis.Timeline(container, items, options);

}
function redrawTimeline(id, dataModel) {
    var container = document.getElementById(id);
    items = new vis.DataSet(dataModel);
    var options = {

        locales: {
            // create a new locale (text strings should be replaced with localized strings)
            mylocale: {
                current: 'current',
                time: 'time',
                deleteSelected: 'Delete selected'
            }
        },

        // use the new locale
        locale: 'mylocale',
        //max: "2022-11-21", -> Set a maximum Date for the visible range
        //min: "2022-12-21"  -> Set a minimum Date for the visible range
    };

    timeline.destroy();
    timeline = new vis.Timeline(container, items, options);
}

window.mapHelper = {
    init: function (id, dotnetHelper) {
        console.log("init:");
        const element = document.querySelector("#" + id);
        const info = document.querySelector(".il-isimleri");
        element.addEventListener("mouseover", function (event) {
            if (event.target.tagName === "path" && event.target.parentNode.id !== "guney-kibris") {
                info.innerHTML = ["<div>", event.target.parentNode.getAttribute("data-iladi"), "</div>"].join("");
            }
        });

        element.addEventListener("mousemove", function (event) {
            const xOffset = 10;
            const yOffset = 25;
            info.style.position = "fixed";
            info.style.top = event.clientY - yOffset + "px";
            info.style.left = event.clientX + xOffset + "px";
        });

        element.addEventListener("mouseout", function (event) {
            info.innerHTML = "";
        });

        element.addEventListener("click", function (event) {
            if (event.target.tagName === "path") {

                const parent = event.target.parentNode;
                console.log("country:", parent);

                const id = parent.getAttribute("id");

                if (id === "guney-kibris") {
                    return;
                }

                let element = document.getElementById(id)

                const turkey = document.getElementById("turkiye")
                const kibris = document.getElementById("kibris")
                let countries = [turkey, kibris];

                countries.forEach(function (countryElement) {
                    let country = { id: countryElement.getAttribute("id"), cities: [] };

                    for (let index = 0; index < countryElement.children.length; index++) {
                        const city = countryElement.children[index];

                        for (let index = 0; index < city.children.length; index++) {
                            city.children[index].style.fill = "#3699FF"
                        }
                    }
                })

                for (let index = 0; index < element.children.length; index++) {
                    element.children[index].style.fill = "#1b61a8"
                }

                //window.location.href = '#' + id + '-' + parent.getAttribute('data-plakakodu');
                //_self.props.selectCity(parent.getAttribute("data-plakakodu"));
                dotnetHelper.invokeMethodAsync("OnCitySelected", parent.getAttribute("data-plakakodu"));
            }
        });
    },

}