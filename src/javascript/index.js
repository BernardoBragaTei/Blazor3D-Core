import { Vector3 } from "three";
import * as THREE from "three";
import Viewer3D from "./Viewer/Viewer3D";

let viewer3d;
let viewers = [];

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
  viewers.push(viewer3d);
}

function getViewerById(viewerId) {
    if (!viewerId) {
    console.warn("Provided viewerId is null or undefined.");
    return null;
  }
  const viewer = viewers.find(v => 
    v.options.viewerSettings.containerId === viewerId);
  if (!viewer) {
    console.warn(`Viewer with id ${viewerId} not found.`);
    return null;
  }
  return viewer;
}


export function updateScene(json, viewerId = null) {
  const viewer = getViewerById(viewerId);
  const sceneOptions = JSON.parse(json);
  viewer.updateScene(sceneOptions);
}

export function addToScene(json, viewerId = null) {
  const viewer = getViewerById(viewerId);
  const sceneOptions = JSON.parse(json);
  viewer.addToScene(sceneOptions);
}

export function removeByUuid(guid, viewerId = null) {
  const viewer = getViewerById(viewerId);
  return viewer.removeByUuid(guid);
}

export function selectByUuid(guid, viewerId = null) {
  const viewer = getViewerById(viewerId);
  return viewer.selectByUuid(guid);
}

export function clearScene(viewerId = null) {
  const viewer = getViewerById(viewerId);
  viewer.clearScene();
}

export function import3DModel(json, viewerId = null) {
  const viewer = getViewerById(viewerId);
  const settings = JSON.parse(json);
  return JSON.stringify(viewer.import3DModel(settings));
}

export function importSprite(json, viewerId = null) {
  const viewer = getViewerById(viewerId);
  const settings = JSON.parse(json);
  return JSON.stringify(viewer.importSprite(settings));
}

export function setCameraPosition(position, lookAt, viewerId = null) {
  const viewer = getViewerById(viewerId);
  viewer.setCameraPosition(position, lookAt);
}

export function updateCamera(json , viewerId = null) {
  const viewer = getViewerById(viewerId);
  const options = JSON.parse(json);
  viewer.updateCamera(options);
}

export function showCurrentCameraInfo() {
  viewer3d.showCurrentCameraInfo();
}

export function updateOrbitControls(json){
  const options = JSON.parse(json);
  viewer3d.updateOrbitControls(options);
}

export function getSceneItemByGuid(guid, viewerId = null) {
  const viewer = getViewerById(viewerId);
  const item = viewer.getSceneItemByGuid(guid);
  return JSON.stringify(item);
}

export function toggleVisibilityByUuid(guid, visible, viewerId = null) {
  const viewer = getViewerById(viewerId);
  return viewer.toggleVisibilityByUuid(guid, visible);
}

export function zoomToFit(padding = 1.2, viewerId = null) {
  const viewer = getViewerById(viewerId);
  return viewer.zoomToFit(padding);
}

export function getScreenCoordinates(modelCoordinates, viewerId = null) {
  const viewer = getViewerById(viewerId);
  const vector = new THREE.Vector3(
    modelCoordinates.x, modelCoordinates.y, modelCoordinates.z);
  vector.project(viewer.camera);
  const canvas = viewer.renderer.domElement;

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
