const express = require('express');
const { getModulesByType, getTypes, getModules, getCategorys, getExtensions, UpdateModule } = require('../controllers/mongoController');

const router = express.Router();

router.get('/modules', getModules);
router.get('/types', getTypes);
router.get('/categorys', getCategorys);
router.get('/extensions', getExtensions);
router.get('/modules-by-type', getModulesByType);
router.put('/module-update', UpdateModule);

module.exports = router;
