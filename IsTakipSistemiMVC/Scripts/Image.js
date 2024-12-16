function openLargeImage(img) {
    var modal = document.getElementById('largeImageModal');
    var largeImg = document.getElementById('largeImage');
    largeImg.src = img.src; // Resim kaynaðýný güncelle
    modal.style.display = 'block'; // Modali göster
}

function closeLargeImage() {
    var modal = document.getElementById('largeImageModal');
    modal.style.display = 'none'; // Modali gizle
}