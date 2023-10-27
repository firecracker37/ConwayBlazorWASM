window.getInnerWidth = function () {
    return window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
}

window.registerResizeEvent = function (dotNetReference) {
    window.addEventListener('resize', function () {
        dotNetReference.invokeMethodAsync('OnWindowResize');
    });
}

window.requestAnimationLoop = (dotnetHelper) => {
    const loop = () => {
        dotnetHelper.invokeMethodAsync('Render');
        requestAnimationFrame(loop);
    };
    loop();
};

// Adds a click listener to the main canvas
window.addMainCanvasClickListener = (dotnetHelper) => {
    const canvases = document.querySelectorAll('canvas');
    const mainCanvas = canvases[0]; // Assuming the main canvas is the first
    if (mainCanvas) {
        mainCanvas.addEventListener('click', (event) => {
            const rect = mainCanvas.getBoundingClientRect();
            const x = event.clientX - rect.left;
            const y = event.clientY - rect.top;
            dotnetHelper.invokeMethodAsync('CanvasClick', x, y);
        });
    }
};

// Add mouse and touch listeners to the main canvas
window.addCanvasInteractionListener = (dotnetHelper) => {
    const canvases = document.querySelectorAll('canvas');
    const mainCanvas = canvases[0]; // Assuming the main canvas is the first
    if (mainCanvas) {
        mainCanvas.addEventListener('mousemove', () => {
            dotnetHelper.invokeMethodAsync('HandleCanvasInteraction');
        });
        mainCanvas.addEventListener('touchstart', () => {
            dotnetHelper.invokeMethodAsync('HandleCanvasInteraction');
        });
    }
};

// Transfers content from the buffer canvas to the main canvas
window.transferBufferContent = () => {
    const canvases = document.querySelectorAll('canvas');
    const mainCanvas = canvases[0];
    const bufferCanvas = canvases[1];
    if (mainCanvas && bufferCanvas) {
        const mainContext = mainCanvas.getContext('2d');
        mainContext.drawImage(bufferCanvas, 0, 0);
    }
};

// Transfers content from the grid canvas to the main canvas
window.transferGridContent = () => {
    const canvases = document.querySelectorAll('canvas');
    const mainCanvas = canvases[0];
    const gridCanvas = canvases[2];
    if (mainCanvas && gridCanvas) {
        const mainContext = mainCanvas.getContext('2d');
        mainContext.drawImage(gridCanvas, 0, 0);
    }
};

// Transfers content from the cell canvas to the main canvas
window.transferCellContent = () => {
    const canvases = document.querySelectorAll('canvas');
    const mainCanvas = canvases[0];
    const cellCanvas = canvases[3];
    if (mainCanvas && cellCanvas) {
        const mainContext = mainCanvas.getContext('2d');
        mainContext.drawImage(cellCanvas, 0, 0);
    }
}

// Transfers content from the UI canvas to the main canvas
window.transferUiContent = () => {
    const canvases = document.querySelectorAll('canvas');
    const mainCanvas = canvases[0];
    const UiCanvas = canvases[2];
    if (mainCanvas && UiCanvas) {
        const mainContext = mainCanvas.getContext('2d');
        mainContext.drawImage(UiCanvas, 0, 0);
    }
}

// Add similar transfer functions for the cell and UI canvases as needed

