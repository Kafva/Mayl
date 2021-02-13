<template>
    <select id="labelSelect" v-model="selected" @change="emitEmailTableEvent">
      <option v-for="label in labels" :key="label.id">
        {{ label }}
      </option>
    </select>
</template>

<script>
import {CONFIG} from '../src/config.js'

export default {
    name: 'label-select',
    props: {},
    data: function() {
        return {
            labels: CONFIG.labels,
            selected: CONFIG.defaultLabel
        };
    },
    
    methods:
    {
        emitEmailTableEvent()
        // Emit a signal to the EmailTable, notifying it that  
        // label has changed and that it should reload 
        {
          this.$root.$emit(CONFIG.reloadInboxEvent, "",  this.selected);
        }
    }
}
</script>

<style scoped>
select
{
  margin-left: 15px;
}
</style>
