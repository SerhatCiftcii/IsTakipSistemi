function openLargeImage(img) {
    var modal = document.getElementById('largeImageModal');
    var largeImg = document.getElementById('largeImage');
    largeImg.src = img.src; // Resim kayna��n� g�ncelle
    modal.style.display = 'block'; // Modali g�ster
}

function closeLargeImage() {
    var modal = document.getElementById('largeImageModal');
    modal.style.display = 'none'; // Modali gizle
}