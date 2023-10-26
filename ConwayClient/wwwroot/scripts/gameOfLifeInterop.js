window.requestAnimationLoop = (dotnetHelper) => {
    const loop = () => {
        dotnetHelper.invokeMethodAsync('Render');
        requestAnimationFrame(loop);
    };
    loop();
};

window.addCanvasClickListener = (dotnetHelper) => {
    const canvas = document.querySelector('canvas');
    if (canvas) {
        canvas.addEventListener('click', (event) => {
            const rect = canvas.getBoundingClientRect();
            const x = event.clientX - rect.left;
            const y = event.clientY - rect.top;
            dotnetHelper.invokeMethodAsync('CanvasClick', x, y);
        });
    }
};

window.createBufferCanvas = (width, height) => {
    const bufferCanvas = new OffscreenCanvas(width, height);
    return bufferCanvas;
};

window.transferGridContent = () => {
    var canvases = document.querySelectorAll('canvas');
    if (canvases.length < 3) {
        console.error('Expected at least three canvas elements but found ' + canvases.length);
        return;
    }

    var mainCanvas = canvases[0];
    var gridCanvas = canvases[1]; // Canvas for grid lines
    var mainContext = mainCanvas.getContext('2d');

    // Transfer grid lines
    mainContext.drawImage(gridCanvas, 0, 0);
};

window.transferBufferContent = () => {
    var canvases = document.querySelectorAll('canvas');
    if (canvases.length < 3) {
        console.error('Expected at least three canvas elements but found ' + canvases.length);
        return;
    }

    var mainCanvas = canvases[0];
    var bufferCanvas = canvases[2]; // Canvas for buffering game state
    var mainContext = mainCanvas.getContext('2d');

    // Transfer game state from buffer canvas
    mainContext.drawImage(bufferCanvas, 0, 0);
};

function togglePlayPauseIcon(isGameRunning) {
    var iconElement = document.getElementById('playPauseIcon');
    if (isGameRunning) {
        iconElement.innerText = 'pause';
    } else {
        iconElement.innerText = 'play_arrow';
    }
};

function resizeAllMaterializeTextareas() {
    var textareas = document.querySelectorAll('textarea.materialize-textarea');
    for (var i = 0; i < textareas.length; i++) {
        M.textareaAutoResize(textareas[i]);
    }
};