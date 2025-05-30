import * as THREE from "three";
import Transforms from "../Utils/Transforms";
import SceneBuilder from "./SceneBuilder";

class GroupBuilder {

    static BuildGroup(options, scene) {
        const group = new THREE.Group();
        group.uuid = options.uuid;
        group.userData.elemId = options.uuid;
        options.children.forEach((childOptions) => {
            // todo: changes for text here (see Viewer.SetScene)
            var child = SceneBuilder.BuildChild(childOptions, scene);
            if (child) {
                group.add(child);
            }
        });
        Transforms.setPosition(group, options.position);
        Transforms.setRotation(group, options.rotation);
        Transforms.setScale(group, options.scale);
        return group;
    }
}

export default GroupBuilder;