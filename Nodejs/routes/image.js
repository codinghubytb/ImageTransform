const express = require('express');
const { Convert, Resize, Cropper, Filter, Rotate, Compression, Watermark } = require('../controllers/imagetransform');
const multer = require('multer');
const upload = multer({ storage: multer.memoryStorage() });

const router = express.Router();

// Pour les fonctions qui nécessitent un fichier
router.post('/convert', upload.single('image'), Convert);
router.post('/resize', upload.single('image'), Resize);
/*router.post('/cropper', upload.single('image'), Cropper);*/
router.post('/filter', upload.single('image'), Filter);
router.post('/rotate', upload.single('image'), Rotate);
router.post('/compression', upload.single('image'), Compression);
router.post('/watermark', upload.fields([{ name: 'image' }, { name: 'watermark' }]), Watermark);


module.exports = router;
