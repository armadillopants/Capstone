using UnityEditor;

public class ModelImporterScript : AssetPostprocessor {
	
	void OnPreprocessModel(){
		ModelImporter importer = assetImporter as ModelImporter;
		//importer.globalScale = 1f;
		importer.meshCompression = ModelImporterMeshCompression.High;
	}
}
