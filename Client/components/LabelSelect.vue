<template>
    <select id="labelSelect" v-model="selected" @change="emitEmailTableEvent">
      <option v-for="label in labels" :key="label.id">
        {{ label }}
      </option>
    </select>
</template>

<script>
import {CONFIG} from '../src/config.js'
import * as Functions  from '../src/functions.js';

export default {
    name: 'label-select',
    props: {},
    data: function() {
        return {
            labels: [],
            selected: CONFIG.defaultLabel
        };
    },
   
    created(){ this.fetchLabels(); },

    methods:
    {
        emitEmailTableEvent()
        // Emit a signal to the EmailTable, notifying it that  
        // label has changed and that it should reload 
        {
          this.$root.$emit(CONFIG.reloadInboxEvent, "",  this.selected);
        },
        
        fetchLabels: async function()
        {
          let res = await fetch(`/${await Functions.waitForAccount(CONFIG.accountSelector)}/labels`, {
            method: "GET",
          });
          
          let body = null;
          try 
          { 
            body = await res.text();
            
            // Decode from base64 and then translate \u sequences into actual
            // glyphs with JSON.parse()
            this.labels = JSON.parse(atob(body));
          }
          catch (e) { console.error(e); }

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
