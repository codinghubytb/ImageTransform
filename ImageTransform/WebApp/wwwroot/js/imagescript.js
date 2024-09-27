window.addEventListener('resize', function () {
    DotNet.invokeMethodAsync('WebApp', 'OnBrowserResize', window.innerWidth, window.innerHeight);
});

window.getDivWidth = (elementId) => {
    var element = document.getElementById(elementId);
    if (element) {
        return element.offsetWidth;
    }
    return null;
};
function adjustImageHeightById(imageId, angle) {
    const radians = angle * (Math.PI / 180); // Convert degrees to radians
    const image = document.getElementById(imageId);

    // Get the natural width and height of the image
    const naturalWidth = image.naturalWidth;
    const naturalHeight = image.naturalHeight;

    // Calculate the bounding box height after rotation
    const newHeight = Math.abs(naturalHeight * Math.cos(radians)) + Math.abs(naturalWidth * Math.sin(radians));

    // Apply the calculated height to the image
    return newHeight;
};
window.getImageDimensions = async function (imageUrl) {
    return new Promise((resolve, reject) => {
        const img = new Image();
        img.onload = function () {
            resolve([img.width, img.height]); // Retourne un tableau avec la largeur et la hauteur
        };
        img.onerror = function () {
            reject("Error loading image");
        };
        img.src = imageUrl; // Charge l'image à partir de l'URL
    });
};

function downloadImage(imageUrl, fileName) {
    // Créer un élément <a>
    const link = document.createElement('a');

    // Définir l'URL de l'image à télécharger
    link.href = imageUrl;

    // Définir le nom du fichier à télécharger
    link.download = fileName;

    // Ajouter l'élément <a> au DOM
    document.body.appendChild(link);

    // Simuler un clic pour télécharger l'image
    link.click();

    // Supprimer l'élément <a> du DOM
    document.body.removeChild(link);
}
