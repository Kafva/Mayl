<template>
    <div id="bar">
      <i id="reloadButton" v-on:click="reloadClick" class="btn nf nf-mdi-reload"></i>
      <label-select></label-select>
    </div>
</template>
<script>

import LabelSelect from '../components/LabelSelect.vue';
import { CONFIG } from '../src/config';

export default {
  name: 'bar',
  
  components: 
  {
    'label-select': LabelSelect
  },
  
  computed: 
  {
    username: function()
    {
      return "FranzKafque@gmail.com";
    }
  },

  methods:
  {
    reloadClick: function(event)
    {
      // Omit the label argument to have the EmailTable use its current value
      this.$root.$emit(CONFIG.reloadInboxEvent, null);
    }
  }
}

</script>

<style>

#reloadButton
{
  font-size: 20px;
}

#bar
{
  position: fixed;
  text-align: center;
  left: 70%;
  width: 30%;
  height: fit-content;
  
  display: flex;
  flex-direction: row;
  flex-wrap: nowrap;
  
  align-items: flex-start;
  justify-content: flex-end;
}

</style>
