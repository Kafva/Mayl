<template>
    <h1>{{ username }}</h1>
</template>
<script>

export default {
  name: 'bar',
  prop: { user: Object },
  computed: 
  {
    username: function()
    {
      return "Blink";
    }
  }
}

</script>

<style>

h1{ font-size: 14px; float: right; padding-right: 20px; }

#bar
{
    background-color: var(--bar);
    opacity: 1.0;
    position: fixed;

    /* The bar only contains text elements (besides the floated <select> elements) */
    text-align: center;

    width: 100%;
    height: fit-content;
    min-width: 50%;
    z-index: 10;
}

</style>
