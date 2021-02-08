<template>
    <select v-model="selected" selected="defaultLabel" @change="emitEmailTableEvent">
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
            defaultLabel: CONFIG.defaultLabel,
            selected: CONFIG.defaultLabel
        };
    },
    
    methods:
    {
        emitEmailTableEvent()
        // Emit a signal to the EmailTable, notifying it that  
        // label has changed and that it should reload 
        {
          this.$root.$emit(CONFIG.reloadInboxEvent, this.selected);
        }
    }
}
</script>

<style>

select
{
  display: inline-block;
  background-color: var(--transbkg);
  color: var(--text);
  max-width: 120px;
  text-overflow: ellipsis;
  margin-left: 10px;
  overflow: hidden;
}

</style>

