import { Vector3 } from "three";
import * as THREE from "three";
import Viewer3D from "./Viewer/Viewer3D";

let viewer3d;

const resizeObserver = new ResizeObserver((entries) => {
  viewer3d.onResize();
});

export function loadViewer(json) {
  const options = JSON.parse(json);
  let container = document.getElementById(options.viewerSettings.containerId);
  if (!container) {
    console.warn("Container not found!");
    return;
  }
  resizeObserver.observe(container);
  viewer3d = new Viewer3D(options, container);
}

export function updateScene(json) {
  const sceneOptions = JSON.parse(json);
  viewer3d.updateScene(sceneOptions);
}

export function addToScene(json) {
  const sceneOptions = JSON.parse(json);
  viewer3d.addToScene(sceneOptions);
}

export function removeByUuid(guid) {
  return viewer3d.removeByUuid(guid);
}

export function selectByUuid(guid) {
  return viewer3d.selectByUuid(guid);
}

export function clearScene() {
  viewer3d.clearScene();
}

export function import3DModel(json) {
  const settings = JSON.parse(json);
  return JSON.stringify(viewer3d.import3DModel(settings));
}

export function importSprite(json) {
  const settings = JSON.parse(json);
  return JSON.stringify(viewer3d.importSprite(settings));
}

export function setCameraPosition(position, lookAt) {
  viewer3d.setCameraPosition(position, lookAt);
}

export function updateCamera(json) {
  const options = JSON.parse(json);
  viewer3d.updateCamera(options);
}

export function showCurrentCameraInfo() {
  viewer3d.showCurrentCameraInfo();
}

export function updateOrbitControls(json){
  const options = JSON.parse(json);
  viewer3d.updateOrbitControls(options);
}

export function getSceneItemByGuid(guid) {
  const item = viewer3d.getSceneItemByGuid(guid);
  return JSON.stringify(item);
}

export function toggleVisibilityByUuid(guid, visible) {
  return viewer3d.toggleVisibilityByUuid(guid, visible);
}

export function getScreenCoordinates(modelCoordinates){
  const vector = new THREE.Vector3(
    modelCoordinates.x, modelCoordinates.y, modelCoordinates.z);
  vector.project(viewer3d.camera);
  const canvas = viewer3d.renderer.domElement;

      // If outside -1 to 1 range, it's off-screen
    if (vector.x < -1 || vector.x > 1 || vector.y < -1 || vector.y > 1 || vector.z < 0) {
        return null; // Point is not visible
    }

    const widthHalf = canvas.clientWidth / 2;
    const heightHalf = canvas.clientHeight / 2;

    return new THREE.Vector2(
        (vector.x * widthHalf) + widthHalf,
        (-vector.y * heightHalf) + heightHalf
    );
}
