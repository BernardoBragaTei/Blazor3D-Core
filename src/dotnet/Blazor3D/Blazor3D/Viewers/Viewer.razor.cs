﻿using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using HomagGroup.Blazor3D.Settings;
using HomagGroup.Blazor3D.Scenes;
using HomagGroup.Blazor3D.Cameras;
using HomagGroup.Blazor3D.Controls;
using Newtonsoft.Json;
using HomagGroup.Blazor3D.Maths;
using HomagGroup.Blazor3D.Objects;
using HomagGroup.Blazor3D.Lights;
using HomagGroup.Blazor3D.ComponentHelpers;
using HomagGroup.Blazor3D.Events;
using Newtonsoft.Json.Linq;
using HomagGroup.Blazor3D.Core;
using HomagGroup.Blazor3D.Materials;

namespace HomagGroup.Blazor3D.Viewers
{
    /// <summary>
    /// <para>HomagGroup.Blazor3D viewer component.</para>
    /// </summary>
    public sealed partial class Viewer : IDisposable
    {
        private IJSObjectReference bundleModule = null!;

        private delegate void SelectedObjectStaticEventHandler(Object3DStaticArgs e);
        private static event SelectedObjectStaticEventHandler ObjectSelectedStatic = null!;

        private delegate void LoadedObjectStaticEventHandler(Object3DStaticArgs e);
        private static event LoadedObjectStaticEventHandler ObjectLoadedStatic = null!;

        private delegate void HoveredObjectStaticEventHandler(RayIntersectArgs e);
        private static event HoveredObjectStaticEventHandler ObjectHoveredStatic = null!;

        private event LoadedObjectEventHandler ObjectLoadedPrivate = null!;

        public delegate void OrbitChangeStaticEventHandler(ContainerStaticArgs e);
        public static event OrbitChangeStaticEventHandler OrbitChangedStatic = null!;

        public delegate void HoveredObjectsEventHandler(RayIntersectArgs intersectArgs);

        /// <summary>
        /// Raises when the orbit corntrols raises the change event.
        /// </summary>
        public event EventHandler OrbitChanged = null!;

        /// <summary>
        /// Raises when user selects object by mouse clicking inside viewer area.
        /// </summary>
        public event SelectedObjectEventHandler ObjectSelected = null!;

        /// <summary>
        /// Raises when user hover elements.
        /// </summary>
        public event HoveredObjectsEventHandler ObjectsHovered = null!;

        /// <summary>
        /// Raises after complete loading of imported file content.
        /// </summary>
        public event LoadedObjectEventHandler ObjectLoaded = null!;

        
        /// <summary>
        /// Raises after JavaScript module is completely loaded.
        /// </summary>
        public event LoadedModuleEventHandler JsModuleLoaded = null!;
       

        /// <summary>
        /// <para><see cref="Settings.ViewerSettings"/> parameter of the component.</para>
        /// </summary>
        [Parameter]
        public ViewerSettings ViewerSettings { get; set; } = new ViewerSettings();

        /// <summary>
        /// <para><see cref="Scenes.Scene"/> parameter of the component. Default is empty scene.</para>
        /// </summary>
        [Parameter]
        public Scene Scene { get; set; } = new Scene();

        /// <summary>
        /// <para>If true and there is no children objects in the scene, then adds the default lights and box mesh. Default value is false.</para>
        /// </summary>
        [Parameter]
        public bool UseDefaultScene { get; set; } = false;

        /// <summary>
        /// <para><see cref="PerspectiveCamera"/> used to display the scene.</para>
        /// </summary>
        [Parameter]
        public Camera Camera { get; set; } = new PerspectiveCamera() { Position = new Vector3(3, 3, 3) };

        /// <summary>
        /// <para><see cref="Controls.OrbitControls"/> used to rotate, pan and scale the view.</para>
        /// </summary>
        [Parameter]
        public OrbitControls OrbitControls { get; set; } = new OrbitControls();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                ObjectSelectedStatic += OnObjectSelectedStatic;
                ObjectLoadedStatic += OnObjectLoadedStatic;
                ObjectLoadedPrivate += OnObjectLoadedPrivate;
                ObjectHoveredStatic += OnObjectHoveredStatic;
                OrbitChangedStatic += OnOrbitChangedStatic;

                bundleModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import",
                    "./_content/Blazor3D/js/bundle.js")
                .AsTask();

                if (UseDefaultScene && !Scene.Children.Any())
                {
                    AddDefaultScene();
                }

                var json = JsonConvert.SerializeObject(new
                {
                    Scene = Scene,
                    ViewerSettings = ViewerSettings,
                    Camera = Camera,
                    OrbitControls = OrbitControls,
                },
                SerializationHelper.GetSerializerSettings());

                await bundleModule.InvokeVoidAsync("loadViewer", json, this.ViewerSettings.ContainerId);
                await OnModuleLoaded();
            }
        }

        private void OnOrbitChangedStatic(ContainerStaticArgs e)
        {
            if (ViewerSettings.ContainerId == e.ContainerId)
            {
                OrbitChanged?.Invoke(null, new EventArgs());
            }
        }

        /// <summary>
        /// Updates scene changes.
        /// </summary>
        /// <returns>Task</returns>
        public async Task UpdateScene()
        {
            var json = JsonConvert.SerializeObject(Scene, SerializationHelper.GetSerializerSettings());
            await bundleModule.InvokeVoidAsync("updateScene", json, this.ViewerSettings.ContainerId);
        }

        public async Task AddToSceneAsync(Object3D obj3d)
        {
            var json = JsonConvert.SerializeObject(obj3d, SerializationHelper.GetSerializerSettings());
            await bundleModule.InvokeVoidAsync("addToScene", json, this.ViewerSettings.ContainerId);
        }

        /// <summary>
        /// <para>Sets the camera position to specified <see cref="Vector3"/> value.</para>
        /// </summary>
        /// <param name="position">New <see cref="Vector3"/> position.</param>
        /// <param name="lookAt">New <see cref="Vector3"/> camera target point coordinates.</param>
        /// <returns>Task</returns>
        public async Task SetCameraPositionAsync(Vector3 position, Vector3 lookAt = null!)
        {
            await bundleModule.InvokeVoidAsync("setCameraPosition", position, lookAt, this.ViewerSettings.ContainerId);
        }

        

        /// <summary>
        /// Apply updated camera settings to viewer.
        /// </summary>
        /// <returns>Task</returns>
        public async Task UpdateCamera(Camera camera)
        {
            Camera = camera;
            var json = JsonConvert.SerializeObject(Camera, SerializationHelper.GetSerializerSettings());
            await bundleModule.InvokeVoidAsync("updateCamera", json, this.ViewerSettings.ContainerId);
        }

        /// <summary>
        /// Prints information about current camera and orbit controls into browser console.
        /// </summary>
        /// <returns>Task</returns>
        public async Task ShowCurrentCameraInfo()
        {
            await bundleModule.InvokeVoidAsync("showCurrentCameraInfo", this.ViewerSettings.ContainerId);
        }

        /// <summary>
        /// Apply updated orbit controls to viewer.
        /// </summary>
        /// <param name="orbitControls">new orbit controls</param>
        /// <returns>Task</returns>
        public async Task UpdateOrbitControls(OrbitControls orbitControls)
        {
            OrbitControls = orbitControls;
            var json = JsonConvert.SerializeObject(OrbitControls, SerializationHelper.GetSerializerSettings());
            await bundleModule.InvokeVoidAsync("updateOrbitControls", json, this.ViewerSettings.ContainerId);
        }

        /// <summary>
        /// Sets the zoom to fit the scene.
        /// </summary>
        /// <param name="padding">Padding to be applied (1 = no padding)</param>
        /// <returns>Task</returns>
        public async Task ZoomToFit(double padding = 1.2)
        {
            await bundleModule.InvokeVoidAsync("zoomToFit", padding, this.ViewerSettings.ContainerId);
        }

        [JSInvokable]
        public static Task ReceiveHoveredObjectData(RayIntersectArgs intersectArgs)
        {
            if(intersectArgs.IntersectData == null || intersectArgs.IntersectData.Length == 0)
            {
                return Task.CompletedTask;
            }
            ObjectHoveredStatic?.Invoke(intersectArgs);
            return Task.CompletedTask;
        }

        [JSInvokable]
        public static Task<string> ReceiveSelectedObjectUUID(string containerId, string uuid)
        {
            var guid = string.IsNullOrWhiteSpace(uuid) ? Guid.Empty : Guid.Parse(uuid);
            var result = containerId + uuid;
            ObjectSelectedStatic?.Invoke(new Object3DStaticArgs()
            {
                ContainerId = containerId,
                UUID = guid,
            });
            return Task.FromResult(result);
        }

        [JSInvokable]
        public static Task ReceiveLoadedObjectUUID(string containerId, string uuid)
        {
            var result = containerId + uuid;
            ObjectLoadedStatic?.Invoke(new Object3DStaticArgs()
            {
                ContainerId = containerId,
                UUID = new Guid(uuid),
            });
            return Task.CompletedTask;
        }

        [JSInvokable]
        public static Task InvokeOrbitChange(string containerId)
        {
            OrbitChangedStatic?.Invoke(new ContainerStaticArgs()
            {
                ContainerId = containerId,
            });
            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes object from scene by it's unique identifier.
        /// </summary>
        /// <param name="uuid">Object's unique identifier.</param>
        /// <returns>Task</returns>
        public async Task RemoveByUuidAsync(Guid uuid)
        {
            var result = await bundleModule.InvokeAsync<bool>("removeByUuid", uuid, this.ViewerSettings.ContainerId);
            if (result)
            {
                ChildrenHelper.RemoveObjectByUuid(uuid, Scene.Children);
            }
        }

        /// <summary>
        /// <para>Selects object in scene by it's unique identifier</para>
        /// </summary>
        /// <param name="uuid">Unique identifier of object to select</param>
        /// <returns>Task</returns>
        public async Task SelectByUuidAsync(Guid uuid)
        {
            await bundleModule.InvokeVoidAsync("selectByUuid", uuid, this.ViewerSettings.ContainerId);
        }

        /// <summary>
        /// <para>Toggles the object visibility on the scene</para>
        /// </summary>
        /// <param name="uuid">Unique identifier of object to select</param>
        /// <param name="visibility">Whether the object mus become visible or not</param>
        /// <returns>Task</returns>
        public async Task ToggleVisibilityByUuidAsync(Guid uuid, bool visibility)
        {
            await bundleModule.InvokeVoidAsync("toggleVisibilityByUuid", uuid, visibility, this.ViewerSettings.ContainerId);
        }

        /// <summary>
        /// Clears scene.
        /// </summary>
        /// <returns>Task</returns>
        public async Task ClearSceneAsync()
        {
            await bundleModule.InvokeVoidAsync("clearScene", this.ViewerSettings.ContainerId);
            Scene.Children.Clear();
        }

        /// <summary>
        /// Gets the screen coordinates of the point if that one is visible by the camera.
        /// </summary>
        public async Task<Vector2?> GetScreenCoordinates(Vector3 modelCoordinates)
        {
            var correctedCoords = new Vector3(modelCoordinates.X, modelCoordinates.Z, -modelCoordinates.Y);
            return await bundleModule.InvokeAsync<Vector2?>("getScreenCoordinates", correctedCoords, this.ViewerSettings.ContainerId);
        }

        /// <summary>
        /// <para>Imports 3D model to scene.</para>
        /// </summary>
        /// <param name="settings"><see cref="ImportSettings"/> Settings that will be applied during 3D model file importing.</param>
        /// <returns>Guid of the loaded item</returns>
        public async Task<Guid> Import3DModelAsync(ImportSettings settings)
        {
            settings.Uuid = settings.Uuid ?? Guid.NewGuid();
            settings.Material = settings.Material ?? new MeshStandardMaterial();
            var json = JsonConvert.SerializeObject(settings, SerializationHelper.GetSerializerSettings());
            await bundleModule.InvokeVoidAsync("import3DModel", json, this.ViewerSettings.ContainerId);
            return settings.Uuid.Value;
        }

        /// <summary>
        /// <para>Imports sprite to scene.</para>
        /// </summary>
        /// <param name="settings"> Settings that will be applied during 2D sprite file importing.</param>
        /// <returns>Guid of the loaded item</returns>
        public async Task<Guid> ImportSpriteAsync(SpriteImportSettings settings)
        {
            settings.Uuid = settings.Uuid ?? Guid.NewGuid();
            settings.Material = settings.Material ?? new SpriteMaterial();
            var json = JsonConvert.SerializeObject(settings, SerializationHelper.GetSerializerSettings());
            await bundleModule.InvokeVoidAsync("importSprite", json, this.ViewerSettings.ContainerId);
            return settings.Uuid.Value;
        }

        /// <summary>
        /// <para>Recursively finds object by it's uuid in collection.</para>
        /// </summary>
        /// <param name="uuid">Object's uuid</param>
        /// <param name="children">Children collection. Usually it's Scene.Children</param>
        /// <returns>Found object or null</returns>
        public static Object3D? GetObjectByUuid(Guid uuid, List<Object3D> children)
        {
            return ChildrenHelper.GetObjectByUuid(uuid, children);
        }

        private async Task OnModuleLoaded()
        {
            LoadedModuleEventHandler handler = JsModuleLoaded;

            if (handler == null)
            {
                return;
            }

            Delegate[] invocationList = handler.GetInvocationList();
            Task[] handlerTasks = new Task[invocationList.Length];

            for (int i = 0; i < invocationList.Length; i++)
            {
                handlerTasks[i] = ((LoadedModuleEventHandler)invocationList[i])();
            }

            await Task.WhenAll(handlerTasks);
        }

        //todo: move to helper
        private void AddDefaultScene()
        {
            Scene.Add(new AmbientLight());
            Scene.Add(new PointLight()
            {
                Position = new Vector3
                {
                    X = 1,
                    Y = 3,
                    Z = 0
                }
            });
            Scene.Add(new Mesh());
        }

        private void OnObjectSelectedStatic(Object3DStaticArgs e)
        {
            if (ViewerSettings.ContainerId == e.ContainerId)
            {
                ObjectSelected?.Invoke(new Object3DArgs() { UUID = e.UUID });
            }
        }

        public async Task ToggleViewerSelection(bool enable)
        {
            await bundleModule.InvokeVoidAsync("toggleViewerSelection", enable, this.ViewerSettings.ContainerId);
        }

        private void OnObjectHoveredStatic(RayIntersectArgs e)
        {
            if (ViewerSettings.ContainerId == e.ContainerId)
            {
                ObjectsHovered?.Invoke(e);
            }
        }

        private void OnObjectLoadedStatic(Object3DStaticArgs e)
        {
            if (ViewerSettings.ContainerId == e.ContainerId)
            {
                ObjectLoadedPrivate?.Invoke(new Object3DArgs() { UUID = e.UUID });
            }
        }

        //todo: move to children helper
        private List<Object3D> ParseChildren(JToken? children)
        {
            var result = new List<Object3D>();
            if (children?.Type != JTokenType.Array)
            {
                return result;
            }

            foreach (JToken child in children)
            {
                var c = child as JObject;
                if (c == null)
                {
                    continue;
                }

                var type = c.Property("type")?.Value.ToString();
                var name = c.Property("name")?.Value.ToString() ?? string.Empty;
                var uuid = c.Property("uuid")?.Value.ToString() ?? string.Empty;
                if (type == "Mesh")
                {
                    var mesh = new Mesh()
                    {
                        Name = name,
                        Uuid = Guid.Parse(uuid)
                    };
                    result.Add(mesh);
                }

                if (type == "Group")
                {
                    var ch = c.Property("children")?.Value;
                    var childrenResult = ParseChildren(ch);
                    var group = new Group
                    {
                        Name = name,
                        Uuid = Guid.Parse(uuid),
                    };
                    group.Children.AddRange(childrenResult);
                }
            }
            return result;
        }

        private async Task OnObjectLoadedPrivate(Object3DArgs e)
        {
            var json = await bundleModule.InvokeAsync<string>("getSceneItemByGuid", e.UUID, this.ViewerSettings.ContainerId);
            if (json.Contains("\"type\":\"Group\""))
            {
                var jobject = JObject.Parse(json);
                var name = jobject.Property("name")?.Value.ToString() ?? string.Empty;
                var uuidstr = jobject.Property("uuid")?.Value.ToString() ?? string.Empty;
                var children = jobject.Property("children")?.Value;
                var childrenResult = ParseChildren(children);
                var group = new Group
                {
                    Name = name,
                    Uuid = Guid.Parse(uuidstr),
                };
                group.Children.AddRange(childrenResult);

                Scene.Children.Add(group);
                ObjectLoaded?.Invoke(new Object3DArgs() { UUID = e.UUID });
            }

            if (json.Contains("\"type\":\"Mesh\""))
            {
                var mesh = JsonConvert.DeserializeObject<Mesh>(json);
                if (mesh != null)
                {
                    Scene.Children.Add(mesh);
                    ObjectLoaded?.Invoke(new Object3DArgs() { UUID = e.UUID });
                }
            }

            if (json.Contains("\"type\":\"Sprite\""))
            {
                var sprite = JsonConvert.DeserializeObject<Sprite>(json);
                if (sprite != null)
                {
                    Scene.Children.Add(sprite);
                    ObjectLoaded?.Invoke(new Object3DArgs() { UUID= e.UUID });
                }
            }
        }

        public void Dispose()
        {
            ObjectSelectedStatic -= OnObjectSelectedStatic;
            ObjectLoadedStatic -= OnObjectLoadedStatic;
            ObjectLoadedPrivate -= OnObjectLoadedPrivate;
            ObjectHoveredStatic -= OnObjectHoveredStatic;
            OrbitChangedStatic -= OnOrbitChangedStatic;
        }
    }
}
