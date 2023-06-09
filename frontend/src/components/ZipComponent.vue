<template>
    <div class="row">
        <div class="col-sm-12">
            <div class="card mb-2 shadow">
                <div class="card-body">
                    <h5 class="card-title">Upload .ZIP</h5>
                    <div class="form-group file-group">
                        <input class="form-control" type="file" id="zip" v-on:change="setZip">
                        <div class="float-right d-flex justify-content-end zip-btns">
                            <button type="button" class="btn btn-primary" @click="validateZip">Validate</button>
                            <button type="button" class="btn btn-success" :disabled="isUploadDisabled" @click="uploadZip">Upload</button>
                        </div>
                    </div>
                </div>
        </div>
        </div>
    </div>
    <div class="row">
        <div class="col-sm-6">
            <div class="card shadow">
                <div class="card-body">
                    <h5 class="card-title">Zips</h5>
                    <ul class="zip-list">
                        <li v-for="item in list">
                            <span>{{ item }}</span>
                            <div class="form-group zip-list-btns">
                                <button type="button" class="btn btn-sm btn-info" @click="getZipContent(item)"><i class="fa fa-file-archive-o" aria-hidden="true"></i></button>
                                <button type="button" class="btn btn-sm btn-danger" @click="deleteZipMoalOpen(item)"><i class="fa fa-trash"></i></button>
                            </div>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
        <div class="col-sm-6">
            <div class="card shadow">
                <div class="card-body">
                    <h5 class="card-title">Content</h5>
                    <div class="crumbs alert alert-secondary">
                        <span v-for="crumb in crumbs">/ {{ crumb }}</span>
                    </div>
                    <Node :data="data"></Node>
                    <ul class="text-danger">
                        <li v-for="error in errors">{{ error }}</li>
                    </ul>
                </div>
            </div>
        </div>
    </div>

    <div class="modal" tabindex="-1" role="dialog" id="confirm-delete">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Delete .ZIP</h5>
                </div>
                <div class="modal-body">
                    <p>Are you sure you want to delete selected .zip?</p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary" @click="deleteZip">Confirm</button>
                    <button type="button" class="btn btn-secondary" data-dismiss="modal" @click="deleteZipModalClose">Close</button>
                </div>
            </div>
        </div>
    </div>
  </template>

<script lang="ts">
import axios from 'axios';
import Node from './Node.vue';
import { toDisplayString } from 'vue';
export default {
    components: { Node },
    data() {
        return {
            list: [],
            data: [],
            zip: null,
            errors: [],
            selectedZip: null,
            validated: false,
            selectedNode: null
            
        }
    },
    created() {
        axios.get('http://localhost:7115/api/zip/get')
            .then(response => {
                this.list = response.data.zips;
            })
            .catch(error => {
                console.log(error);
            });
    },
    mounted() {
        this.emitter.on('set-node', record => {
            this.selectedNode = record;
        });
    },
    methods: {
        getZipContent: function(name) {
            axios.get('http://localhost:7115/api/zip/get/' + name)
            .then(response => {
                this.data = response.data.content;
                toastr.success("Contents successfully retrieved", "Success");
            })
            .catch(error => {
                console.log(error);
            });
        },
        setZip: function(e) {
            let files = e.target.files || e.dataTransfer.files;
            if (!files.length) {
                return;
            }

            this.zip = files[0];
            console.log(this.zip);
        },
        validateZip: function() {
            let formData = new FormData();
            formData.append("zip", this.zip);

            axios.post('http://localhost:7115/api/zip/validate', formData)
            .then(response => {
                this.data = response.data.content;
                this.errors = response.data.errors;
                this.validated = true;

                toastr.success("Zip validated", "Success");
            })
            .catch(error => {
                console.log(error);
            });
        },
        deleteZipMoalOpen(zip) {
            this.selectedZip = zip;
            $("#confirm-delete").modal("show");
        },
        deleteZipModalClose() {
            $("#confirm-delete").modal("hide");
        },
        deleteZip() {
            axios.post('http://localhost:7115/api/zip/delete/' + this.selectedZip)
            .then(response => {
                this.list = this.list.filter(r => r != this.selectedZip);
                this.deleteZipModalClose();
                toastr.success("Zip successfully deleted", "Success");
            })
            .catch(error => {
                console.log(error);
            }); 
        },
        uploadZip() {
            let formData = new FormData();
            formData.append("zip", this.zip);

            axios.post('http://localhost:7115/api/zip/upload', formData)
            .then(response => {
                this.list.push(response.data.zip);
            })
            .catch(error => {
                console.log(error);
            });
        },
        getParents(data, parent, crumbs = []) {
            data.forEach(r => {
                console.log(r.name, parent);
                if (r.name == parent) {
                    crumbs.push(r.name);
                    parent = r.parent;
                    crumbs = this.getParents(this.data, parent, crumbs);
                } else {
                    crumbs = this.getParents(r.children, parent, crumbs);
                }
            });

            return crumbs;
        }
    },
    computed: {
        isUploadDisabled() {
            if (!this.validated) return true;
            if (this.errors.length > 0) return true;

            return false;
        },
        crumbs() {
            if (!this.selectedNode) return [];
            let crumbs = this.getParents(this.data, this.selectedNode.parent, []);
            crumbs.push(this.selectedNode.name);

            return crumbs;
        }
    }
}

</script>

<style scoped>
ul.zip-list {
    list-style: none;
}
ul.zip-list li {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin: 10px;
}
.zip-list-btns .btn-danger {
    margin-left: 5px;
}
.text-danger {
    list-style-type: "*";
}
.text-danger li {
    padding-left: 5px;
}
.zip-btns button {
    margin: 5px 0 0 5px;
}
</style>