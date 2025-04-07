// // Variable to track the state
// var isFieldLeft = false;

// window.onload = function () {
//   document.querySelector('.overlay').style.opacity = 0;
//   setTimeout(function () {
//     document.querySelector('.overlay').style.display = 'none';
//   }, 1000);

//   // Ensure elements are present before modifying styles
//   var container = document.getElementById('container');
//   var page = document.getElementById('page');
//   var pageContent = document.getElementById('pageContent');

//   if (container && page && pageContent) {
//     container.style.width = '20%';
//     page.style.width = '78%';
//   }
// }

// function openPage(title, fieldNumber) {
//   // Disable hover effect for all fields
//   document.querySelectorAll('.field').forEach(function (field) {
//     field.style.transition = '';
//     field.style.flex = '0.875';
//     field.style.width = '17.5%';
//     field.style.pointerEvents = 'auto';
//     field.style.opacity = 1;
//   });

//   // Resize the clicked field to 20% width
//   var clickedField = document.querySelector('.field[data-number="' + fieldNumber + '"]');
//   clickedField.style.transition = 'width 0.5s ease';
//   clickedField.style.width = '20%';

//   // Hide all fields except the clicked one if not in the left position
//   if (!isFieldLeft) {
//     document.querySelectorAll('.field:not([data-number="' + fieldNumber + '"])').forEach(function (field, index) {
//       field.style.transition = 'opacity 0.5s ease-out';
//       field.style.opacity = 0;

//       // Add delay for a fade-in effect
//       setTimeout(function () {
//         field.style.pointerEvents = 'none';
//       }, 500 * (index + 1));
//     });

   
//     showPopupContainer(fieldNumber);
//   } else {
//     // Enable pointer events for all fields when returning to the original position
//     document.querySelectorAll('.field').forEach(function (field, index) {
//       field.style.transition = 'opacity 0.5s ease-out';
//       field.style.opacity = 1;

//       // Remove delay after fading in
//       setTimeout(function () {
//         field.style.pointerEvents = 'auto';
//       }, 500 * (index + 1));
//     });


//     hidePopupContainer();
//   }

//   // Move the clicked field to the position of the first field
//   var firstField = document.querySelector('.field[data-number="1"]');

//   // Calculate the offset of the first field
//   var firstFieldRect = firstField.getBoundingClientRect();
//   var offsetLeft = firstFieldRect.left;
//   var offsetTop = firstFieldRect.top;

//   // Calculate the offset of the clicked field
//   var clickedFieldRect = clickedField.getBoundingClientRect();
//   var offsetLeftClicked = clickedFieldRect.left;
//   var offsetTopClicked = clickedFieldRect.top;

//   // Calculate the difference in offset
//   var offsetLeftDiff = offsetLeft - offsetLeftClicked;
//   var offsetTopDiff = offsetTop - offsetTopClicked;

//   // Apply the translation to the clicked field
//   clickedField.style.transition = 'transform 0.5s ease-out, opacity 0.5s ease-out';
//   clickedField.style.transform = 'translate(' + offsetLeftDiff + 'px, ' + offsetTopDiff + 'px)';
//   clickedField.style.opacity = 1; // Ensure opacity is set to 1


//   document.getElementById('pageContent').innerText = title;

//   isFieldLeft = !isFieldLeft;
// }

// function showPopupContainer(clickedFieldNumber) {
//   var popupContainer = document.querySelector('.popup-container');
//   popupContainer.style.width = '80%'; 

//   // Create and append popup textboxes
//   var titles = ['Describe issue', 'Needed by', 'Je li ovo', 'fakat', 'radi'];
//   for (var i = 0; i < 5; i++) {
//     // Add titles above the textboxes
//     var popupTitle = document.createElement('div');
//     popupTitle.className = 'popup-title';
//     popupTitle.innerText = titles[i];
//     popupContainer.appendChild(popupTitle);

//     var popupTextbox = document.createElement('input');
//     popupTextbox.className = 'popup-textbox';
//     popupTextbox.type = 'text';
//     popupTextbox.placeholder = 'Text ' + (i + 1);
//     popupContainer.appendChild(popupTextbox);
//   }

//   // Add submit button
//   var submitButton = document.createElement('div');
//   submitButton.className = 'submit-button';
//   submitButton.innerText = 'Submit';
//   submitButton.onclick = function () {
//     submitPopup();
//   };
//   popupContainer.appendChild(submitButton);
// }

// function hidePopupContainer() {
//   var popupContainer = document.querySelector('.popup-container');
//   popupContainer.style.width = '0';
//   // Remove popup textboxes and titles from the DOM
//   var popupTextboxes = document.querySelectorAll('.popup-textbox, .popup-title');
//   popupTextboxes.forEach(function (popupTextbox) {
//     popupTextbox.remove();
//   });


//   var submitButton = document.querySelector('.submit-button');
//   if (submitButton) {
//     submitButton.remove();
//   }
// }

// function submitPopup() {
  
//   alert('Popup submitted!');
// }

// const toggleButton = document.getElementsByClassName('toggle-button')[0];
// const navBarLinks = document.getElementsByClassName('navbar-links')[0];

// toggleButton.addEventListener('click', () => {
//     navBarLinks.classList.toggle('active');
// })


// document.querySelectorAll('.department-btn').forEach(button => {
//   button.addEventListener('click', function() {
//       document.querySelectorAll('.department-btn').forEach(btn => btn.classList.remove('active'));
//       this.classList.add('active');
//   });
// });