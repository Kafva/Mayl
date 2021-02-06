<template>
    <div id="bar">
      <h1>{{ username }}</h1>
      <label-select></label-select>
    </div>
</template>
<script>

import LabelSelect from '../components/LabelSelect.vue';

export default {
  name: 'bar',
  prop: { user: Object },
  
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
  }
}

</script>

<style>

h1 
{ 
  display: inline-block;
  font-size: 14px; 
  padding-right: 20px; 
}

#bar
{
  position: fixed;
  left: 80%;
  text-align: center;
  width: 20%;
  height: fit-content;
  
  display: flex;
  flex-direction: row;
  flex-wrap: nowrap;
  
  align-items: flex-start;
  justify-content: flex-end;

}

</style>
