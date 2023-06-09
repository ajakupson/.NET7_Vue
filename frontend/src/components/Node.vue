<template>
    <ul>
        <li v-for="record in data" @click.stop="setNode(record)">
            <i v-if="record.children.length" class="fa toggler" :class="record.opened ? 'fa-minus-square' : 'fa-plus-square'" aria-hidden="true" @click="toggle(record)"></i>
            <i v-if="record.children.length" class="fa fa-folder-o" aria-hidden="true"></i>
            <i v-if="!record.children.length" class="fa fa-file-o" aria-hidden="true"></i>
            <span class="ml-2">{{ record.name }}</span>
            <Node :data="record.children" v-show="record.opened"></Node>
        </li>
    </ul>
</template>

<script lang="ts">
export default {
    name: "Node",
    props: ['data'],
    data() {
        return {

        }
    },
    mounted() {

    },
    computed: {

    },
    methods: {
        toggle(record) {
            record.opened = !record.opened;
        },
        setNode(record) {
            this.emitter.emit("set-node", record);
        }
    }
}
</script>

<style scoped>
ul {
    border-left: 1px dashed gray;
    list-style-type: "------" !important;
}
ul li {
    margin: 5px;
}
ul li::before {
    width: 20px;
    height: 20px;
    border: 1px dashed gray;
}
ul li i {
    margin-right: 5px;
}

li i.toggler {
    cursor: pointer;
}
</style>