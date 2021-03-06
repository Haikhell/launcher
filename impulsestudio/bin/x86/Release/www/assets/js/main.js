$(document).ready(function () {

    CefSharp.BindObjectAsync("callbackObject");

    $('.startwindow__slider').slick({
        infinite: true,
        slidesToShow: 3,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 2000,
    });


    // Select
    $('.startwindow__servers_select').each(function () {
        const _this = $(this),
            selectOption = _this.find('option'),
            selectOptionLength = selectOption.length,
            selectedOption = selectOption.filter(':selected'),
            duration = 450;

        _this.hide();
        _this.wrap('<div class="startwindow__servers_select"></div>');
        $('<div>', {
            class: 'startwindow__servers_selectnew',
            text: _this.children('option:disabled').text()
        }).insertAfter(_this);

        const selectHead = _this.next('.startwindow__servers_selectnew');
        $('<div>', {
            class: 'startwindow__servers_selectnew_list'
        }).insertAfter(selectHead);

        const selectList = selectHead.next('.startwindow__servers_selectnew_list');
        for (let i = 1; i < selectOptionLength; i++) {
            $('<div>', {
                class: 'startwindow__servers_selectnew_item',
                html: $('<span>', {
                    text: selectOption.eq(i).text()
                })
            })
                .attr('data-value', selectOption.eq(i).val())
                .appendTo(selectList);
        }

        const selectItem = selectList.find('.startwindow__servers_selectnew_item');
        selectList.slideUp(0);
        selectHead.on('click', function () {
            if (!$(this).hasClass('on')) {
                $(this).addClass('on');
                selectList.slideDown(duration);

                selectItem.on('click', function () {
                    let chooseItem = $(this).data('value');

                    $(this)
                        .parent(".startwindow__servers_selectnew_list")
                        .parent(".startwindow__servers_select")
                        .val(chooseItem).attr('selected', 'selected');

                    selectHead.text($(this).find('span').text());
                    selectList.slideUp(duration);
                    selectHead.removeClass('on');
                });

            } else {
                $(this).removeClass('on');
                selectList.slideUp(duration);
            }
        });
    });

});

document.oncontextmenu = function()
{
    return false;
};

function RemoteAction(text)
{
    /*if(text=='startplay')
        showDownload();*/

    callbackObject.message(text);
}

function changePercent(percent)
{
    //document.getElementById("playbutton").innerHTML= percent;
    
    //$("#playbutton").text(percent);

    /*var txt = 'Скачиваем... ' + percent + '%';

    $("#startwindow__load").text(txt);*/
    //document.getElementById("startwindow__load").style.display = "block";

    $("#startwindow__load").text(percent);
}

function showDownload()
{
    document.getElementById("startwindow__load").style.display = "block";
}