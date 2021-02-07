<template>
    <select v-model="selected" selected="defaultLabel" @change="setSelected">
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
        setSelected()
        // Emit a signal to the EmailTable, notifying it that  
        // label has changed and that it should reload 
        {
            this.$root.$emit("reloadInbox", this.selected);
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

