const connectToDatabase = require('../config/db');

async function getModules(req, res) {
    const db = await connectToDatabase();
    const collection = db.collection('module');

    try {
        const data = await collection.find({Enabled: true}).toArray();
        res.json(data);
    } catch (error) {
        console.error(error);
        res.status(500).json({ message: 'Erreur lors de la récupération des données' });
    }
}

async function getTypes(req, res) {
    const db = await connectToDatabase();
    const collection = db.collection('type');

    try {
        const data = await collection.find({}).toArray();
        res.json(data);
    } catch (error) {
        console.error(error);
        res.status(500).json({ message: 'Erreur lors de la récupération des données' });
    }
}

async function getCategorys(req, res) {
    const db = await connectToDatabase();
    const collection = db.collection('category');

    try {
        const data = await collection.find({}).toArray();
        res.json(data);
    } catch (error) {
        console.error(error);
        res.status(500).json({ message: 'Erreur lors de la récupération des données' });
    }
}

async function getExtensions(req, res) {
    const db = await connectToDatabase();
    const collection = db.collection('extension');

    try {
        const data = await collection.find({}).toArray();
        res.json(data);
    } catch (error) {
        console.error(error);
        res.status(500).json({ message: 'Erreur lors de la récupération des données' });
    }
}

async function getModulesByType(req, res) {
    const db = await connectToDatabase();
    const collection = db.collection('module');
    const typeId = req.query.type;  // Obtenir l'identifiant du type depuis le paramètre de requête

    try {
        const data = await collection.find({ Type: typeId , Enabled : true}).toArray();
        res.json(data);
    } catch (error) {
        console.error(error);
        res.status(500).json({ message: 'Erreur lors de la récupération des modules par type' });
    }
}


async function UpdateModule(req, res) {
    const db = await connectToDatabase();
    const collection = db.collection('module');
    const { GuId, Title, Description, DateCreated, Path, Icon,  Category, Type, Visit, Enabled } = req.body;

    try {
        // Vérifier si GuidIdea est fourni
        if (!GuId) {
            return res.status(400).json({ message: 'GuidId est requis' });
        }

        // Préparer l'objet de mise à jour
        const updateFields = {};
        if (Title) updateFields.Title = Title;
        if (Description) updateFields.Description = Description;
        if (DateCreated) updateFields.DateCreated = new Date(DateCreated);
        if (Path) updateFields.Path = Path;
        if (Icon) updateFields.Icon = Icon;
        if (Category) updateFields.Category = Category;
        if (Type) updateFields.Type = Type;
        if (Visit !== undefined) updateFields.Visit = Visit; // Assurez-vous que Visit peut être 0 ou false
        if (Enabled !== undefined) updateFields.Enabled = Enabled; // Assurez-vous que Enabled peut être 0 ou false

        // Exécuter la mise à jour
        const result = await collection.updateOne(
            { GuId: GuId },
            { $set: updateFields }
        );

        // Vérifier si la mise à jour a été un succès
        if (result.matchedCount > 0) {
            if (result.modifiedCount > 0) {
                res.status(200).json({ message: 'Idea mise à jour avec succès' });
            } else {
                res.status(200).json({ message: 'Aucune modification n\'a été apportée' });
            }
        } else {
            res.status(404).json({ message: 'Aucune idée trouvée avec ce GuidIdea' });
        }
    } catch (error) {
        console.error(error);
        res.status(500).json({ message: error.message });
    }
}

module.exports = {
    getModules,
    getTypes,
    getCategorys,
    getExtensions,
    getModulesByType,
    UpdateModule
};
