(() => {
  document.addEventListener('DOMContentLoaded', () => {
    const ENTER_URI = 'Please Enter a URI';
    const URI_EXISTS = 'This uri is already exisis!';
    const ERROR_OCCURED = 'There is an error while prerforming request';
    const DISPLAY = {
      BLOCK: 'block',
      NONE: 'none',
    };

    const registerButton = document.getElementById('register');
    const webhookURI = document.getElementById('webhook');
    const webhookType = document.getElementById('webhooktype');
    const successBox = document.getElementById('alertSuccess');
    const dangerBox = document.getElementById('alertDanger');
    const dangerMessage = document.getElementById('dangerMessage');
    const successMessage = document.getElementById('successMessage');

    registerButton.addEventListener('click', () => {
      successBox.style.display = DISPLAY.NONE;
      dangerBox.style.display = DISPLAY.NONE;

      if (!webhookURI.value) {
        dangerMessage.innerHTML = ENTER_URI;
        dangerBox.style.display = DISPLAY.BLOCK;
      } else {
        fetch('https://localhost:5001/api/WebhookSubscription', {
          method: 'post',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            webhookUri: webhookURI.value,
            webhookType: webhookType.value,
          }),
        }).then((response) => {
          if (response.status === 204) {
            dangerBox.style.display = DISPLAY.BLOCK;
            dangerBox.innerHTML = URI_EXISTS;
            return Promise.resolve();
          }
          return response.json();
        }).then((content) => {
          if (!content) return;

          successMessage.innerHTML = `Webhook Registered please use secret: ${content.secret} to validate inbound request`;
          successBox.style.display = DISPLAY.BLOCK;
        }).catch((error) => {
          dangerBox.style.display = DISPLAY.BLOCK;
          dangerMessage.innerHTML = ERROR_OCCURED;
          console.error('An error has been occured', error);
        });
      }
    });
  })
})();