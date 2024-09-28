const { Double } = require('mongodb');
const sharp = require('sharp');

// Fonction pour convertir l'image dans un autre format
async function Convert(req, res) {
  try {
    const { format, iscompression } = req.body;

    // Liste complète des formats supportés
    const supportedFormats = [
      'jpeg',
      'png',
      'webp',
      'tiff',
      'avif',
      'gif',
      'raw'
    ];

    console.log(req.file.buffer.format);
    if (!supportedFormats.includes(format)) {
      return res.status(400).json({ error: 'Format non pris en charge.' });
    }

    let conversion;
    if(format == "jpeg"){

      conversion = await sharp(req.file.buffer).toFormat(format, { compressionLevel: 9 }).toBuffer();

    }else{
      
      conversion = await sharp(req.file.buffer).toFormat(format, { compressionLevel: 9 }).toBuffer();

    }


    // Déterminez le type MIME en fonction du format
    const mimeTypes = {
      'jpeg': 'image/jpeg',
      'png': 'image/png',
      'webp': 'image/webp',
      'tiff': 'image/tiff',
      'avif': 'image/avif',
      'gif': 'image/gif',
      'raw': 'image/raw'
    };

    res.set('Content-Type', mimeTypes[format]);
    res.json(createResponse(conversion.toString('base64'), format, null));
  } catch (error) {
    console.error('Erreur lors de la conversion de l\'image:', error);
    res.status(500).json(createResponse(null, null, error.message || 'Erreur lors de la conversion de l\'image'));
  }
}


// Fonction pour redimensionner l'image
async function Resize(req, res) {
  try {
    const width = parseInt(req.body.width);
    const height = parseInt(req.body.height);
    const iscompression = parseInt(req.body.iscompression);

    if (isNaN(width) || isNaN(height)) {
      return res.status(400).json({ error: 'Width et height doivent être des nombres.' });
    }

    const resizedImageBuffer = await sharp(req.file.buffer).resize(width, height, { fit: 'fill' }).toBuffer();

    res.set('Content-Type', req.file.mimetype);
    res.json(createResponse(resizedImageBuffer.toString('base64'),  req.file.mimetype.split('/')[1], null));
  } catch (error) {
    console.error('Erreur lors du redimensionnement de l\'image:', error);
    res.status(500).json(createResponse(null, null, 'Erreur lors du redimensionnement de l\'image'));
  }
}
/*
// Fonction pour recadrer l'image
async function Cropper(req, res) {
  try {
    const left = parseInt(req.body.left);
    const top = parseInt(req.body.top);
    const width = parseInt(req.body.width);
    const height = parseInt(req.body.height);
    const iscompression = parseInt(req.body.iscompression);

    if (isNaN(left) || isNaN(top) || isNaN(width) || isNaN(height)) {
      return res.status(400).json({ error: 'Left, top, width, et height doivent être des nombres.' });
    }

    const imageBuffer = await sharp(req.file.buffer).extract({ left, top, width, height }).toBuffer();
 
    res.set('Content-Type', req.file.mimetype);
    res.json(createResponse(imageBuffer.toString('base64'),  req.file.mimetype.split('/')[1], null));
  } catch (error) {
    console.error('Erreur lors du recadrage de l\'image:', error);
    res.status(500).json(createResponse(null, null, 'Erreur lors du recadrage de l\'image'));
  }
}*/

// Fonction pour faire pivoter l'image
async function Rotate(req, res) {
  try {
    const angle = parseInt(req.body.angle);
    const iscompression = req.body.iscompresssion;
  
    if (isNaN(angle)) {
      return res.status(400).json({ error: 'Angle doit être un nombre.' });
    }

    let rotatedImageBuffer;

    rotatedImageBuffer = await sharp(req.file.buffer)
    .rotate(angle)
    .toBuffer();

    res.set('Content-Type', req.file.mimetype);
    res.json(createResponse(rotatedImageBuffer.toString('base64'),  req.file.mimetype.split('/')[1], null));
  } catch (error) {
    console.error('Erreur lors de la rotation de l\'image:', error);
    res.status(500).json(createResponse(null, null, 'Erreur lors de la rotation de l\'image'));
  }
}

// Fonction pour compresser l'image
async function Compression(req, res) {
  try {
    const quality = parseInt(req.body.quality);

    if (isNaN(quality)) {
      return res.status(400).json({ error: 'Quality doit être un nombre.' });
    }

    const compressedImageBuffer = await sharp(req.file.buffer)
      .jpeg({ quality })
      .toBuffer();
    res.set('Content-Type', 'image/jpeg');
    res.json(createResponse(compressedImageBuffer.toString('base64'),  "jpeg", null));
  } catch (error) {
    console.error('Erreur lors de la compression de l\'image:', error);
    res.status(500).json(createResponse(null, null, 'Erreur lors de la compression de l\'image'));
  }
}


async function Watermark(req, res) {
  try {
    // Vérifiez si les fichiers sont présents dans req.files
    if (!req.files || !req.files['image'] || !req.files['watermark']) {
      return res.status(400).json({ error: 'Image et filigrane requis.' });
    }

    const imageBuffer = req.files['image'][0].buffer;
    const watermarkBuffer = req.files['watermark'][0].buffer;

    // Vérifiez que les fichiers ne sont pas vides
    if (!imageBuffer.length || !watermarkBuffer.length) {
      return res.status(400).json({ error: 'L\'image ou le filigrane est vide.' });
    }

    // Vérifiez les types MIME
    const imageMimeType = req.files['image'][0].mimetype;
    const watermarkMimeType = req.files['watermark'][0].mimetype;

    if (!['image/jpeg', 'image/png', 'image/webp'].includes(imageMimeType)) {
      return res.status(400).json({ error: 'Le format de l\'image n\'est pas pris en charge.' });
    }

    if (!['image/jpeg', 'image/png', 'image/webp'].includes(watermarkMimeType)) {
      return res.status(400).json({ error: 'Le format du filigrane n\'est pas pris en charge.' });
    }

    // Redimensionner le filigrane pour qu'il ne soit pas disproportionné
    const imageMetadata = await sharp(imageBuffer).metadata();
    
    const widthWatermark = parseFloat(req.body.widthWatermark) || 0.2; // Définir l'opacité avec une valeur par défaut

    console.log(widthWatermark);

    // Redimensionner le filigrane avec transparence
    const watermarkWithOpacity = await sharp(watermarkBuffer)
      .resize({ width: Math.round(imageMetadata.width * widthWatermark) })  // Redimensionner à 20% de la largeur de l'image principale
      .ensureAlpha()
      .toBuffer();

    // Position du filigrane
    const position = req.body.position || 'southeast';

    // Appliquer le filigrane à l'image
    const watermarkedImageBuffer = await sharp(imageBuffer)
      .composite([{ 
        input: watermarkWithOpacity, 
        gravity: position,
        blend: 'overlay', 
      }])
      .toBuffer();

    // Envoyer l'image modifiée
    res.set('Content-Type', imageMimeType);
    res.json(createResponse(watermarkedImageBuffer.toString('base64'), imageMimeType.split('/')[1], null));
  } catch (error) {
    console.error('Erreur lors de l\'ajout du filigrane à l\'image:', error);
    res.status(500).json(createResponse(null, null, 'Erreur lors de l\'ajout du filigrane à l\'image'));
  }
}


const applyBlurFilter = (image, intensity) => {
  return image.blur(intensity);
};

const applyGrayscaleFilter = (image) => {
  return image.grayscale();
};

const applyInvertFilter = (image) => {
  return image.negate();
};

async function Filter(req, res) {
  try {
    if (!req.file) {
      return res.status(400).json({ error: 'Aucun fichier n\'a été envoyé.' });
    }

    const { filterType } = req.body;
    
    const blurIntensity = parseFloat(req.body.blurIntensity);

    if (!filterType) {
      return res.status(400).json({ error: 'Le type de filtre est requis.' });
    }

    let processedImage = sharp(req.file.buffer);

    switch (filterType) {
      case 'grayscale':
        processedImage = applyGrayscaleFilter(processedImage);
        break;
      case 'blur':
        const intensity = blurIntensity;
        processedImage = applyBlurFilter(processedImage, intensity);
        break;
      case 'invert':
        processedImage = applyInvertFilter(processedImage);
        break;
      default:
        return res.status(400).json(createResponse(null, null, 'Erreur lors de l\'application du filtre à l\'image'));
    }

    const filteredImageBuffer = await processedImage.toBuffer();

    res.set('Content-Type', req.file.mimetype);
    res.json(createResponse(filteredImageBuffer.toString('base64'), req.file.mimetype.split('/')[1], null));
  } catch (error) {
    console.error('Erreur lors de l\'application du filtre à l\'image:', error);
    res.status(500).json(createResponse(null, null, 'Erreur lors de l\'application du filtre à l\'image'));
  }
}



function createResponse(base64Data, extension, error = null) {
  return {
    base64Data: base64Data,
    image: base64Data ? `data:image/${extension};base64,${base64Data}` : null,
    format : extension,
    error: error
  };
}


module.exports = {
  Convert,
  Resize,
  Rotate,
  Compression,
  Filter,
  Watermark
};
