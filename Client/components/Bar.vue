<template>
    <div id="bar">
      <img :src="loadingWheel"/>
      <i id="reloadButton" v-on:click="reloadClick" class="btn nf nf-mdi-reload"></i>
      <label-select></label-select>
      <account-select></account-select>
    </div>
</template>
<script>

import LabelSelect from '../components/LabelSelect.vue';
import AccountSelect from '../components/AccountSelect.vue';
import LoadingWheel from '../assets/loading.gif';
import { CONFIG } from '../src/config';

export default {
  name: 'bar',
  
  components: 
  {
    'label-select': LabelSelect,
    'account-select': AccountSelect
  },
  
  data: function()
  {
    return {
      loadingWheel: LoadingWheel
    }
  },

  methods:
  {
    reloadClick: function(event)
    {
      // Omit the label argument to have the EmailTable use its current value
      this.$root.$emit(CONFIG.reloadInboxEvent, "", "");
    }
  }
}

</script>

<style>

#bar > img
{
  display: none;
  width:  30px;
  height: 30px;
  padding-right: 10px;
}

#reloadButton
{
  padding-top: 5px;
  font-size: 20px;
}

#bar
{
  z-index: 900;
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
