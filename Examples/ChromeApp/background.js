chrome.app.runtime.onLaunched.addListener(function() {
  chrome.app.window.create('window.html', 
  {
    'outerBounds': {
      'width': 400,
      'height': 500
    }
  });
});

chrome.runtime.onSuspend.addListener(function() {
  // Do some simple clean-up tasks.
});