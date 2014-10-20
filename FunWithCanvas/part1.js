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

    var canvas = document.getElementById(id + 'Canvas');
    canvas.style.display = "none";
    var context = canvas.getContext('2d');
    var W = canvas.width;
    var H = canvas.height;

    var img = document.getElementById(id + 'Image');
    img.onmouseover = function () {
        img.style.display = "none";
        canvas.style.display = "inherit";

        var gear = new Gear(W / 2, H / 2, 5, 12, "white", "rgba(61, 142, 198, 1)");
        gear.angularSpeed = 36;

        setInterval(function () {
            canvas.width = canvas.width;
            gear.render(context);
        }, 60);
    }
}('gearsTutorialPart1'));