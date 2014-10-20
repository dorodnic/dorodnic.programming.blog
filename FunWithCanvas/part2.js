(function (id) {
    function deg2rad(d) {
        return (2 * Math.PI * d) / 360;
    }

    function rad2deg(r) {
        return (360 * r) / (2 * Math.PI);
    }

    function distance(x1, y1, x2, y2) {
        return Math.sqrt(Math.pow(x1 - x2, 2) + Math.pow(y1 - y2, 2));
    }

    var Gear = function(x, y, connectionRadius, teeth, fillStyle, strokeStyle) {
        // Gear parameters
        this.x = x;
        this.y = y;
        this.connectionRadius = connectionRadius;
        this.teeth = teeth;

        // Render parameters
        this.fillStyle = fillStyle;
        this.strokeStyle = strokeStyle;

        // Calculated properties
        this.diameter = teeth * 4 * connectionRadius; // Each touth is built from two circles of connectionRadius
        this.radius = this.diameter / (2 * Math.PI); // D = 2 PI r

        // Animation properties
        this.phi0 = 0; // Starting angle
        this.angularSpeed = 0; // Speed of rotation in degrees per second
        this.createdAt = new Date(); // Timestamp
    }

    Gear.prototype.render = function (context) {
        // Update rotation angle
        var ellapsed = new Date() - this.createdAt;
        var phiDegrees = this.angularSpeed * (ellapsed / 1000);
        var phi = this.phi0 + deg2rad(phiDegrees); // Current angle

        // Set-up rendering properties
        context.fillStyle = this.fillStyle;
        context.strokeStyle = this.strokeStyle;
        context.lineCap = 'round';
        context.lineWidth = 1;

        // Draw gear body
        context.beginPath();
        for (var i = 0; i < this.teeth * 2; i++) {
            var alpha = 2 * Math.PI * (i / (this.teeth * 2)) + phi;
            // Calculate individual touth position
            var x = this.x + Math.cos(alpha) * this.radius;
            var y = this.y + Math.sin(alpha) * this.radius;

            // Draw a half-circle, rotate it together with alpha
            // On every odd touth, invert the half-circle
            context.arc(x, y, this.connectionRadius, -Math.PI / 2 + alpha, Math.PI / 2 + alpha, i % 2 == 0);
        }
        context.fill();
        context.stroke();

        // Draw center circle
        context.beginPath();
        context.arc(this.x, this.y, this.connectionRadius, 0, 2 * Math.PI, true);
        context.fill();
        context.stroke();
    }

    Gear.prototype.connect = function (x, y) {
        var r = this.radius;
        var dist = distance(x, y, this.x, this.y);

        // To create new gear we have to know the number of its touth
        var newRadius = Math.max(dist - r, 10);
        var newDiam = newRadius * 2 * Math.PI;
        var newTeeth = Math.round(newDiam / (4 * this.connectionRadius));

        // Calculate the ACTUAL position for the new gear, that would allow it to interlock with this gear
        var actualDiameter = newTeeth * 4 * this.connectionRadius;
        var actualRadius = actualDiameter / (2 * Math.PI);
        var actualDist = r + actualRadius; // Actual distance from center of this gear
        var alpha = Math.atan2(y - this.y, x - this.x); // Angle between center of this gear and (x,y)
        var actualX = this.x + Math.cos(alpha) * actualDist; 
        var actualY = this.y + Math.sin(alpha) * actualDist;

        // Make new gear
        var newGear = new Gear(actualX, actualY, this.connectionRadius, newTeeth, this.fillStyle, this.strokeStyle);

        // Adjust new gear's rotation to be in direction oposite to the original
        var gearRatio = this.teeth / newTeeth;
        newGear.angularSpeed = -this.angularSpeed * gearRatio;

        // At time t=0, rotate this gear to be at angle Alpha
        this.phi0 = alpha + (this.phi0 - alpha); // = this.phi0, does nothing, for demonstration purposes only
        newGear.phi0 = alpha + Math.PI + (Math.PI / newTeeth) + (this.phi0 - alpha) * (newGear.angularSpeed / this.angularSpeed);
        // At the same time (t=0), rotate the new gear to be at (180 - Alpha), facing the first gear,
        // And add a half gear rotation to make the teeth interlock
        newGear.createdAt = this.createdAt; // Also, syncronize their clocks


        return newGear;
    }

    var canvas = document.getElementById(id + 'Canvas');
    canvas.style.display = "none";
    var context = canvas.getContext('2d');
    var W = canvas.width;
    var H = canvas.height;

    var img = document.getElementById(id + 'Image');
    img.onmouseover = function () {
        img.style.display = "none";
        canvas.style.display = "inherit";

        var gear = new Gear(W / 2, H / 2, 5, 10, "white", "rgba(61, 142, 198, 1)");
        gear.angularSpeed = 36;
        var gear2 = gear.connect(3 * (W / 4), H / 2);

        // Helper function to translate (x,y) to coordinates relative to the canvas
        function getMousePos(canvas, evnt) {
            var rect = canvas.getBoundingClientRect();
            return {
                x: evnt.clientX - rect.left,
                y: evnt.clientY - rect.top
            };
        }

        canvas.onmousemove = function (evnt) {
            var pos = getMousePos(canvas, evnt);

            var x = Math.min(0.7 * W, Math.max(0.3 * W, pos.x));
            var y = Math.min(0.7 * H, Math.max(0.3 * H, pos.y));

            gear2 = gear.connect(x, y);
        }

        setInterval(function () {
            canvas.width = canvas.width;
            gear.render(context);
            gear2.render(context);
        }, 60);
    }
}('gearsTutorialPart2_3'));