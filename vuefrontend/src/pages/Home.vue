<template>
  <div>
    <div class="overlay"></div>

    <div class="container">
      <div class="field" data-number="1" @click="openPage('ONBOARDING', 1)">
        <div class="text-container">
          <!-- Replace with your actual SVG or icon component -->
          <!-- <svg width="110" height="110" viewBox="...">...</svg> -->
          <div class="text">ONBOARDING</div>
        </div>
      </div>

      <div class="field" data-number="2" @click="openPage('REQUEST SOFTWARE OR EQUIPMENT', 2)">
        <div class="text-container">
          <!-- <svg width="110" height="110" viewBox="...">...</svg> -->
          <div class="text">REQUEST SOFTWARE OR EQUIPMENT</div>
        </div>
      </div>

      <div class="field" data-number="3" @click="openPage('REQUEST SIGNATURE', 3)">
        <div class="text-container">
          <!-- <svg width="110" height="110" viewBox="...">...</svg> -->
          <div class="text">REQUEST SIGNATURE</div>
        </div>
      </div>

      <div class="field" data-number="4" @click="openPage('REPORT ISSUE', 4)">
        <div class="text-container">
          <!-- <svg width="110" height="110" viewBox="...">...</svg> -->
          <div class="text">REPORT ISSUE</div>
        </div>
      </div>

      <div class="field" data-number="5" @click="openPage('OFFBOARDING', 5)">
        <div class="text-container">
          <!-- <svg width="110" height="110" viewBox="...">...</svg> -->
          <div class="text">OFFBOARDING</div>
        </div>
      </div>
    </div>

    <!-- Dynamic content injection area -->
    <div class="popup-container"></div>
    <div class="page" id="page">
      <div class="page-content" id="pageContent"></div>
    </div>
  </div>
</template>

<script setup>
import { onMounted } from 'vue';

const openPage = (pageName, number) => {
  alert(`Opening ${pageName} (box #${number})`);
  // You can replace this with router.push or open a modal
};

onMounted(() => {
  const button1 = document.getElementById('button1');
  const button2 = document.getElementById('button2');

  if (button1 && button2) {
    const toggleSelection = (buttonId) => {
      document.querySelectorAll('.button').forEach(btn => btn.classList.remove('active'));
      document.getElementById(buttonId).classList.add('active');
    };

    button1.addEventListener('click', () => toggleSelection('button1'));
    button2.addEventListener('click', () => toggleSelection('button2'));
  }
});
</script>

<style scoped>
/* @import '../assets/css/custom.css'; */
/* Include your custom styles */
</style>
