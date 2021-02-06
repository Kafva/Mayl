<template>
    <v-select
        id="labelSelect" 
        :options="['INBOX','UNREAD','STARRED']" 
        :value="selected" 
        @input="setSelected"
    ></v-select>
</template>

<script>
import {CONFIG} from '../src/config.js'

// Custom <select> element component 
// https://vue-select.org/guide/options.html#options-prop
import vSelect from 'vue-select';

// Custom version of the packages CSS to have the dropdown work
import '../src/vue-select.css';

export default {
    name: 'label-select',
    props: { selected: String },
    
    components: { vSelect: vSelect },
    methods:
    {
        setSelected(value)
        {
            console.log(value);
        }
    }
}
</script>

<style>
</style>

