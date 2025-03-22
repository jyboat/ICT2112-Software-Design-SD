let customUpload = $('.wrapper')

if (customUpload) {
    customUpload.on('click', function () {
        $('input[name="resourceFile"]').click()
    })

    let poster = document.getElementsByName('resourceFile')[0]

    if (poster) {
        $('input[name="resourceFile"]').on('change', function () {
            previewImg(poster, $('.poster'))
        })
    }
    else {
        $('input[name="resourceFile"]').on('change', function () {
            previewFile(poster)
        })
    }
}

function previewImg(fileInput, img) {
    let file = fileInput.files
    let reader = new FileReader()

    reader.onload = function (e) {
        img.attr('src', e.target.result);
    }

    if (file && file[0]) {
        reader.readAsDataURL(file[0]);
    }

    if ($('.filename')) {
        if (file.length > 0) {
            let fileName = fileInput.value.split('\\')
            $('.filename').html(fileName[fileName.length - 1])
            $('.infos').removeClass('d-none')
            $('.remove').removeClass('d-none')
            $('.card-text').addClass('d-none')
        }
        else {
            $('.infos').addClass('d-none')
            $('.remove').addClass('d-none')
            $('.card-text').removeClass('d-none')
            img.attr('src', '/img/placeholder.jpg')
        }
    }
}

function previewFile(fileInput) {
    let file = fileInput.files

    if ($('.filename')) {
        if (file.length > 0) {
            let fileName = fileInput.value.split('\\')
            $('.filename').html(fileName[fileName.length - 1])
            $('.infos').removeClass('d-none')
            $('.remove').removeClass('d-none')
            $('.card-text').addClass('d-none')
        }
        else {
            $('.infos').addClass('d-none')
            $('.remove').addClass('d-none')
            $('.card-text').removeClass('d-none')
        }
    }
}

if ($('.remove')) {
    $('.remove').on('click', function () {
        removeFile()
    })
}

function removeFile() {
    $('.infos').addClass('d-none')
    $('.remove').addClass('d-none')
    $('.card-text').removeClass('d-none')
    $('input[name="resourceFile"]').val('')
    if ($('.poster')) {
        $('.poster').attr('src', '/img/placeholder.jpg')
    }
}