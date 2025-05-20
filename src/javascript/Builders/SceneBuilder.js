import HelperBuilder from "./HelperBuilder";
import LightBuilder from "./LightBuilder";
import MeshBuilder from "./MeshBuilder";
import GroupBuilder from "./GroupBuilder";
import LineBuilder from "./LineBuilder";
import SpriteBuilder from "./SpriteBuilder";
import TextBuilder from "./TextBuilder";

class SceneBuilder {

  static BuildChild(options, scene) {
    if (options.type == "Text") {      
      return TextBuilder.BuildText(options);
    }
    if (options.type == "Mesh") {
      return MeshBuilder.BuildMesh(options);
    }

    if (options.type == "Line") {
      return LineBuilder.BuildMesh(options);
    }
    
    if (options.type == "SplineCurve"){
      return SplineCurveBuilder.BuildMesh(options)
    }

    if (options.type == "Group") {
      return GroupBuilder.BuildGroup(options);
    }

    if (options.type == "AmbientLight") {
      return LightBuilder.BuildAmbientLight(options);
    }

    if (options.type == "PointLight") {
      return LightBuilder.BuildPointLight(options);
    }

    if (options.type.includes("Helper")) {
      return HelperBuilder.BuildHelper(options, scene);
    }

    if (options.type.includes("Sprite")) {
      return SpriteBuilder.BuildSprite(options);
    }
  }
}

export default SceneBuilder;
