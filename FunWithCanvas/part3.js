var gearbox3dModule = function (canvasName) {

    var Gear = GEARS.Gear;
    var GearsLayer = GEARS.GearsLayer;
    var LayerStackCreationOptions = GEARS.LayerStackCreationOptions;

    var gbCanvas = document.getElementById(canvasName);

    var width = 700;
    var height = 600;
    var gearDepth = 50;

    gbCanvas.onmousemove = function onMouseMove(e) {
        var dx = Math.min(e.offsetX, width * 3 / 4);
        dx = Math.max(dx, width / 4);

        var dy = Math.min(e.offsetY, height * 3 / 4);
        dy = Math.max(dy, height / 4);

        light.position.set(dx - width / 2, height / 2 - dy, 550);
    }

    var scene = new THREE.Scene();
    var camera = new THREE.PerspectiveCamera(50, width / height, 1, 10000);
    camera.position.set(width / 2, -height / 2, 1200);
    camera.lookAt({ x: width / 2, y: -height / 2, z: 0 });

    // LIGHTS
    scene.add(new THREE.AmbientLight(0x444444));
    var light = new THREE.DirectionalLight(0xFFFFFF, 0.8);
    light.position.set(0, 0, 550);
    light.exponent = 0.5;
    light.castShadow = true;
    light.shadowDarkness = 0.5;
    light.shadowMapWidth = light.shadowMapHeight = 2048;
    light.shadowCameraNear = 10;
    light.shadowCameraFar = 5000;
    light.shadowCameraLeft = -5000;
    light.shadowCameraRight = 5000;
    light.shadowCameraTop = 5000;
    light.shadowCameraBottom = -5000;
    scene.add(light);

    var renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });
    renderer.setSize(width, height);
    renderer.setClearColor(0xFFFFFF, 1);
    renderer.shadowMapEnabled = true;
    renderer.shadowMapType = THREE.PCFSoftShadowMap;
    renderer.shadowMapSoft = false;
    renderer.shadowCameraNear = 0;
    renderer.shadowCameraFar = camera.far;
    renderer.shadowCameraFov = 100;
    renderer.shadowMapBias = 0.1;
    renderer.shadowMapDarkness = 0.3;
    renderer.shadowMapWidth = 2048;
    renderer.shadowMapHeight = 2048;

    gbCanvas.appendChild(renderer.domElement);

    var options = new GEARS.LayerStackCreationOptions();
    options.minX = 0;
    options.minY = 0;
    options.count = 5;
    options.qouta = 1000;
    options.maxX = width;
    options.maxY = height;
    options.maxGears = 150;
    options.connectionRadius = 4;

    options.generator.set(60);
    var layers = options.create();

    var group = new THREE.Group();

    var i = 0;
    layers.forEach(function (layer) {
        layer.gears.forEach(function (gear) {
            gear.track();

            gear.group = addShape(gear.getShape(),
                gear.fillStyle.get(), gear.strokeStyle.get(),
                0, 0, 0, 0, 0, -gear.phi, 1);

            gear.group.position.x = gear.x;
            gear.group.position.y = -gear.y;
            gear.group.position.z = i * gearDepth;

            group.add(gear.group);
        });
        i++;
    });
    scene.add(group);

    // Create the ground plane
    var plane = new THREE.Shape();
    plane.moveTo(0, 0);
    plane.lineTo(width, 0);
    plane.lineTo(width, height);
    plane.lineTo(0, height);
    var geometry = new THREE.ShapeGeometry(plane);
    var material = new THREE.MeshLambertMaterial({ color: 0xFFFFFF, wireframe: false });
    var mesh = new THREE.Mesh(geometry, material);
    mesh.position.set(-10000, -10000, -50);
    mesh.rotation.set(0, 0, 0);
    mesh.scale.set(200, 200, 1);
    mesh.receiveShadow = true;
    scene.add(mesh);


    var render = function () {
        requestAnimationFrame(render);

        layers.forEach(function (layer) {
            layer.gears.forEach(function (gear) {
                gear.track(); // Update rotation angles
                gear.group.rotation.z = -gear.phi;
            });
        });

        renderer.render(scene, camera);
    };
    render();

    function addShape(shape, color, strokeColor, x, y, z, rx, ry, rz, s) {
        var group = new THREE.Group();

        // Extrude shape:
        var extrudeSettings = {
            steps: 1,
            amount: gearDepth,
            bevelEnabled: false,
        };
        var geometry = new THREE.ExtrudeGeometry(shape, extrudeSettings);
        var material = new THREE.MeshPhongMaterial({
            color: color,
            polygonOffset: true,
            polygonOffsetFactor: 1.0,
            polygonOffsetUnits: 4.0
        });
        var mesh = new THREE.Mesh(geometry, material);
        mesh.position.set(x, y, z);
        mesh.rotation.set(rx, ry, rz);
        mesh.scale.set(s, s, s);
        mesh.castShadow = true;
        mesh.receiveShadow = true;
        group.add(mesh);

        // Create outline:
        var geometry = shape.createPointsGeometry();
        var material = new THREE.LineBasicMaterial({ linewidth: 10, color: strokeColor, transparent: false });
        var line = new THREE.Line(geometry, material);
        line.position.set(x, y, z + gearDepth);
        line.rotation.set(rx, ry, rz);
        line.scale.set(s, s, s);
        group.add(line);

        return group;
    } 

}("jsGears");